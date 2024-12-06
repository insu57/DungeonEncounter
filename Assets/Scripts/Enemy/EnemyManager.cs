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
        public float Height { get; private set; }
        private WorldUIView _worldUIView;
        private EnemyWorldUIPresenter _uiPresenter;
        public event Action<float,float> OnHealthChanged;
        public event Action OnTest;
        
        private float GetStat(EnemyStatTypes type)
        {
            return _enemyStats.GetValueOrDefault(type, 0);
        }

        private void UpdateHealth(float damage)
        {
            _enemyStats[EnemyStatTypes.Health] -= damage;
            OnHealthChanged?.Invoke(GetStat(EnemyStatTypes.Health),GetStat(EnemyStatTypes.MaxHealth));
        }

        private void DropItem()
        {
            //Money min~max Consumable Chest
            //일단 Instantiate로 생성
            Vector3 pos = transform.position;
            //Money
            float moneyAmount = Random.Range(_dropTable.MoneyRangeStart, _dropTable.MoneyRangeEnd+1);
            GameObject money = Instantiate(_dropTable.MoneyPrefab, pos+Vector3.back, Quaternion.identity);
            money.GetComponent<Money>().SetMoneyAmount(moneyAmount);
            //Consumable
            float consumableChance = _dropTable.ConsumableChance;
            float randomChance = Random.value;
            
            if (randomChance > consumableChance)
            {
                int consumableTotalWeight = _dropTable.ConsumableTotalWeight;
                int randomWeight = Random.Range(0, consumableTotalWeight);
                int cumulativeWeight = 0;
                foreach (var drop in _dropTable.ConsumableItems)
                {
                    cumulativeWeight += drop.dropWeight;
                    if (cumulativeWeight > randomWeight)
                    {
                        Instantiate(drop.dropPrefab, pos+Vector3.left, Quaternion.identity);
                        break;
                    }
                }
            }
            
            

        }
        
        private void Awake()
        {
            _enemyControl = GetComponent<EnemyControl>();
            _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            _dropTable = data.DropTable;
            _type = data.Type;
            
            Height = GetComponent<Collider>().bounds.size.y;
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
        }
        
        private void Update()
        {
            
        
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttack") && _enemyControl.WasDamaged == false)
            {
                //데미지...현재 플레이어 공격력 기준... 공격별 데미지 배수, 스킬 추가 대응필요
                float damage = _playerManager.GetStat(PlayerStatTypes.AttackValue);
                UpdateHealth(damage);
                OnTest?.Invoke();
                float health = GetStat(EnemyStatTypes.Health);
                //test
                if (health <= 0)
                {
                    _enemyControl.IsAttack = false;
                    _enemyControl.IsMove = false;
                    _enemyControl.IsDead = true;
                    DropItem();
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