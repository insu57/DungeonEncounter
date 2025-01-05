using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Enemy
{
    public enum EnemyStatTypes
    {
        Health, MaxHealth, Damage
    }
    
    public class EnemyManager : MonoBehaviour //적
    {
        [SerializeField] private EnemyData data;
        private EnemyDropTable _dropTable;
        private string _type;
        private PlayerManager _playerManager;
        private EnemyControl _enemyControl;
        public EnemyData Data => data;
        private Dictionary<EnemyStatTypes, float> _enemyStats;
        private Collider _enemyCollider;
        public float Height { get; private set; }
        
        private FlashOnHit _flashOnHit;
        private WorldUIView _worldUIView;
        private EnemyWorldUIPresenter _uiPresenter;
        public event Action<float,float> OnHealthChanged;
        public event Action OnDeath;
        private float GetStat(EnemyStatTypes type)
        {
            return _enemyStats.GetValueOrDefault(type, 0);
        }

        private void UpdateHealth(float damage)
        {
            _enemyStats[EnemyStatTypes.Health] -= damage;
            OnHealthChanged?.Invoke(GetStat(EnemyStatTypes.Health),GetStat(EnemyStatTypes.MaxHealth));
        }

        public Vector3 EnemyTargetPos()
        {
            return _playerManager.transform.position;
        }
        
        private void DropItem()
        {
            //Money min~max Consumable Chest
            //드랍테이블 등 데이터 입력 방식 변경 필요
            Vector3 pos = transform.position;
            //Money
            int moneyAmount = Random.Range(_dropTable.MoneyRangeStart, _dropTable.MoneyRangeEnd+1);
            GameObject money = ObjectPoolingManager.Instance
                .GetObjectFromPool(PoolKeys.Money, pos+Vector3.back, Quaternion.identity);
            money.GetComponent<Money>().SetMoneyAmount(moneyAmount);
            //Consumable
            float consumableChance = _dropTable.ConsumableChance;
            float randomChance = Random.value;
            
            if (randomChance < consumableChance)
            {
                int consumableTotalWeight = _dropTable.ConsumableTotalWeight;
                int randomWeight = Random.Range(0, consumableTotalWeight);
                int cumulativeWeight = 0;
                foreach (var drop in _dropTable.ConsumableItems)
                {
                    cumulativeWeight += drop.dropWeight;
                    if (cumulativeWeight <= randomWeight) continue;
                    Instantiate(drop.dropPrefab, pos+Vector3.left, Quaternion.identity);
                    break;
                }
            }
            //Chest
            float chestChance = _dropTable.ChestChance;
            randomChance = Random.value;
            if (randomChance >= chestChance) return;
            float weaponChance = _dropTable.WeaponChance;
            randomChance = Random.value;
            if (randomChance < weaponChance)
            {
                int weaponTotalWeight = _dropTable.WeaponsTotalWeight;
                int randomWeight = Random.Range(0, weaponTotalWeight);
                int cumulativeWeight = 0;
                foreach (var drop in _dropTable.Weapons)
                {
                    cumulativeWeight += drop.dropWeight;
                    if (cumulativeWeight <= randomWeight) continue;
                    //GameObject chest = Instantiate(_dropTable.ChestPrefab, pos+Vector3.right, Quaternion.identity);
                    GameObject chest = ObjectPoolingManager.Instance
                        .GetObjectFromPool(PoolKeys.Chest01, pos+Vector3.right, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(drop.dropPrefab);
                    break;
                }
            }
            else
            {
                int equipmentTotalWeight = _dropTable.EquipmentTotalWeight;
                int randomWeight = Random.Range(0, equipmentTotalWeight);
                int cumulativeWeight = 0;
                foreach (var drop in _dropTable.Equipments)
                {
                    cumulativeWeight += drop.dropWeight;
                    if (cumulativeWeight <= randomWeight) continue;
                    GameObject chest = Instantiate(_dropTable.ChestPrefab, pos+Vector3.right, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(drop.dropPrefab);
                    break;
                }
            }
        }
        
        private void Awake()
        {
            _enemyControl = GetComponent<EnemyControl>();
            _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            _dropTable = data.DropTable;
            _type = data.Type;

            _enemyCollider = GetComponent<Collider>();
            Height = _enemyCollider.bounds.size.y;
            _enemyStats = new Dictionary<EnemyStatTypes, float>
            {
                { EnemyStatTypes.Health , data.MaxHealth},
                { EnemyStatTypes.MaxHealth , data.MaxHealth},
                { EnemyStatTypes.Damage , data.Damage}
            };
            
            _worldUIView = FindObjectOfType<WorldUIView>();
            EnemyHealthBar healthBar = _worldUIView.InitEnemyHealthBar(this);
            _uiPresenter = new EnemyWorldUIPresenter(this, healthBar);
            //EnemyWorldUI초기화
            
            _flashOnHit = GetComponent<FlashOnHit>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttack") && _enemyControl.WasDamaged == false 
                                                 && _enemyControl.IsDead == false)
            {
                _flashOnHit.TriggerFlash();
                float damage = _playerManager.GetFinalAttackValue();
                UpdateHealth(damage);
                float health = GetStat(EnemyStatTypes.Health);
                Debug.Log("Damage: "+damage+" LeftHealth: " + health);
                //death
                if (health <= 0)
                {
                    _enemyControl.IsAttack = false;
                    _enemyControl.IsMove = false;
                    _enemyControl.IsDead = true;
                    DropItem();//아이템 드랍
                    OnDeath?.Invoke();//사망이벤트
                    _enemyCollider.enabled = false;//충돌 비활성
                    _uiPresenter.Dispose();//Presenter Dispose
                }
                
                StartCoroutine(Damaged(0.5f));
            }
        }

        private IEnumerator Damaged(float duration)
        {
            _enemyControl.WasDamaged = true;
            yield return new WaitForSeconds(duration);
            _enemyControl.WasDamaged = false;
        }

        
    
    
    }
}