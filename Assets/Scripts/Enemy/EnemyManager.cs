using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Scriptable_Objects;
using UI;
using UnityEngine;
using UnityEngine.AI;
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
        //private Collider _enemyCollider;
        public float Height { get; private set; }
        private FlashOnHit _flashOnHit;
        [SerializeField] private Collider enemyCollider;
        [SerializeField] private ParticleSystem deathSmokeParticle;
        //할당방식 변경필요?
        private WorldUIView _worldUIView;
        private EnemyWorldUIPresenter _uiPresenter;
        private StageManager _stageManager;
        public PoolKeys key { get; private set; }
        public event Action<float,float> OnHealthChanged;
        public event Action OnDeath;
        public float GetStat(EnemyStatTypes type)
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

        public void InitEnemySpawn(Vector3 spawnPos)
        {
            EnemyHealthBar healthBar = _worldUIView.InitEnemyHealthBar(this);
            _uiPresenter = new EnemyWorldUIPresenter(this, healthBar);
            _stageManager = FindObjectOfType<StageManager>();
            
            var navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = false;
            transform.position = spawnPos;
            navMeshAgent.enabled = true;
        }
        
        private void EnemyDeath()
        {
            _enemyControl.IsAttack = false;
            _enemyControl.IsMove = false;
            _enemyControl.IsDead = true;
            DropItem();//아이템 드랍
            OnDeath?.Invoke();//사망이벤트 
            enemyCollider.enabled = false;//충돌 비활성
            //사망 이펙트 추가
            deathSmokeParticle.Play();
            _uiPresenter.Dispose();//Presenter Dispose
            OnDeath -= _stageManager.HandleEnemyDeath;//StageManager 이벤트 구독해제
            
            StartCoroutine(EnemyReturnToPool(1f));
        }

        public void EnemyOnStageReset()
        {
            _uiPresenter.ReturnHealthBar();
            _uiPresenter.Dispose();
            OnDeath -= _stageManager.HandleEnemyDeath;
            ObjectPoolingManager.Instance.ReturnToPool(data.EnemyKey, gameObject);
        }

        private IEnumerator EnemyReturnToPool(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ObjectPoolingManager.Instance.ReturnToPool(data.EnemyKey, gameObject);
            StopAllCoroutines();
            //transform.position = Vector3.zero;
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
            
            
            int totalRarityWeight = _dropTable.RarityDropWeights
                .Sum(rarityDropWeight => rarityDropWeight.dropWeight);  //레어도 드랍 가중치 총합
            
            //Consumable
            float consumableChance = _dropTable.ConsumableChance;
            float randomChance = Random.value;
            
            int randomWeight = Random.Range(0, totalRarityWeight); 
            int cumulativeWeight = 0;
            
            if (randomChance < consumableChance)
            {
                
                Rarity consumableItemRarity = Rarity.Common; //기본값 Common...예외처리
                foreach (var rarityDropWeight in _dropTable.RarityDropWeights)
                {
                    cumulativeWeight += rarityDropWeight.dropWeight;
                    if(cumulativeWeight <= randomWeight) continue; 
                    consumableItemRarity = rarityDropWeight.rarity;
                    break;
                } //레어도 정하기

                var filteredItems = _stageManager.GetStageConsumableItemData()
                    .Where(item => item.GetRarity() == consumableItemRarity).ToArray();

                if (filteredItems.Length > 0)
                {
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    Instantiate(item.GetItemPrefab(),pos+Vector3.left,Quaternion.identity);
                }
                else
                {
                    //빈 상태면 Uncommon레어도 아이템으로
                    filteredItems = _stageManager.GetStageConsumableItemData().
                        Where(item => item.GetRarity() == Rarity.Uncommon).ToArray();
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    Instantiate(item.GetItemPrefab(),pos+Vector3.right,Quaternion.identity);
                }
            }
            //Chest
            float chestChance = _dropTable.ChestChance;
            randomChance = Random.value;
            if (randomChance >= chestChance) return; //상자 확률값 이상이면 소환x
            float weaponChance = _dropTable.WeaponChance;
            randomChance = Random.value;
            
            randomWeight = Random.Range(0, totalRarityWeight); //랜덤 가중치 0~Total
            cumulativeWeight = 0;
            Rarity itemRarity = Rarity.Common;
            
            foreach (var rarityDropWeight in _dropTable.RarityDropWeights)
            {
                cumulativeWeight += rarityDropWeight.dropWeight;
                if (cumulativeWeight <= randomWeight) continue;
                itemRarity = rarityDropWeight.rarity;
                break;
            }
            
            if (randomChance < weaponChance) //무기 확률값보다 작으면 무기 아니면 장비
            {
                var filteredItems = _stageManager.GetStagePlayerWeaponData()
                    .Where(item => item.GetRarity() == itemRarity).ToArray();

                if (filteredItems.Length > 0)
                {
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    GameObject chest = ObjectPoolingManager.Instance
                        .GetObjectFromPool(PoolKeys.Chest01, pos+Vector3.back, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(item.GetItemPrefab());
                }
                else
                {
                    filteredItems = _stageManager.GetStagePlayerWeaponData()
                        .Where(item => item.GetRarity() == Rarity.Uncommon).ToArray();
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    GameObject chest = ObjectPoolingManager.Instance
                        .GetObjectFromPool(PoolKeys.Chest01, pos+Vector3.back, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(item.GetItemPrefab());
                }
                
            }
            else
            {
                var filteredItems = _stageManager.GetStagePlayerEquipmentData()
                    .Where(item => item.GetRarity() == itemRarity).ToArray();
                if (filteredItems.Length > 0)
                {
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    GameObject chest = ObjectPoolingManager.Instance
                        .GetObjectFromPool(PoolKeys.Chest01, pos+Vector3.back, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(item.GetItemPrefab());
                }
                else
                {
                    filteredItems = _stageManager.GetStagePlayerEquipmentData()
                        .Where(item => item.GetRarity() == Rarity.Uncommon).ToArray();
                    var item = filteredItems[Random.Range(0, filteredItems.Length)];
                    GameObject chest = ObjectPoolingManager.Instance
                        .GetObjectFromPool(PoolKeys.Chest01, pos+Vector3.back, Quaternion.identity);
                    chest.GetComponent<Chest>().SetItem(item.GetItemPrefab());
                }
            }
        }

        private void OnEnable()
        {
            //초기화.
            enemyCollider.enabled = true;
            _enemyStats[EnemyStatTypes.Health] = GetStat(EnemyStatTypes.MaxHealth);
            //Debug.Log("Enemy On Enable...Name: "+gameObject.name+" Position: "+transform.position);
        }

        private void Awake()
        {
            _enemyControl = GetComponent<EnemyControl>();
            _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            _dropTable = data.DropTable;
            _type = data.Type;

            //enemyCollider = GetComponent<Collider>();
            Height = enemyCollider.bounds.size.y;
            _enemyStats = new Dictionary<EnemyStatTypes, float>
            {
                { EnemyStatTypes.Health , data.MaxHealth},
                { EnemyStatTypes.MaxHealth , data.MaxHealth},
                { EnemyStatTypes.Damage , data.Damage}
            };
            
            _worldUIView = FindObjectOfType<WorldUIView>();
            _flashOnHit = GetComponent<FlashOnHit>();
            key = data.EnemyKey;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttack") && _enemyControl.WasDamaged == false 
                                                 && _enemyControl.IsDead == false)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.EnemyDamageSfx);
                _flashOnHit.TriggerFlash();
                float damage = _playerManager.GetFinalAttackValue();
                UpdateHealth(damage);
                float health = GetStat(EnemyStatTypes.Health);
        
                StartCoroutine(Damaged(0.5f));//피격무적시간
                //death
                if (health <= 0)
                {
                    EnemyDeath();
                }
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