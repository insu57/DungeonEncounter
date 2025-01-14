using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Player
{
    //public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem, Damaged ,Global }
    public enum PlayerStatTypes
    {
        Health, MaxHealth,
        Energy, MaxEnergy,
        AttackValue, DefenseValue,
    }

    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField]private PlayerJobData playerJobData;
        private PlayerControl _playerControl;
        [SerializeField]private GameObject playerRightHand;
        [SerializeField]private GameObject playerHead;
        private EnemyMeleeAttack _enemyMeleeAttack;
        private EnemyProjectile _enemyProjectile;
        //[SerializeField] private ItemPrefabData itemPrefabData;
        //public PlayerWeaponData WeaponData { get; private set; }
        //public PlayerEquipmentData EquipmentData { get; private set; }
        private Dictionary<PlayerStatTypes, float> _playerStats; 
        public event Action<PlayerStatTypes,float> OnStatChanged;
        
        public PlayerWeaponData PlayerDefaultWeaponData {get; private set;}
        private GameObject _equippedWeapon;
        private float _weaponAttackValue;
        private WeaponType _playerWeaponType;
        private GameObject _equippedEquipment;
        private float _equipmentDefenseValue;
        private List<ItemEffect> _currentItemEffects;//다른 형태?(아이템별 효과 구분)
        private bool _isRecoverEnergy = false;
        private float _finalAttackValue;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject swordAttackBox;
        [SerializeField] private GameObject attackEffect;
        [SerializeField] private GameObject skillEffect;
        public event Action<int> OnGetMoney;
        public event Action<ItemDataWithID> OnGetItem;
        public event Action<int> OnUseItemQuickSlot;
        public event Action<FloatText, Vector3> OnFloatKey;
        public event Action OnExitFloatKey;
        
        private Camera _mainCamera;
        private void UpdateFinalAttackValue()//각종 효과 구현 진행 필요.
        {
            _finalAttackValue = _weaponAttackValue;
            //+itemEffect...
        }

        public float GetFinalAttackValue()
        {
            _finalAttackValue = _weaponAttackValue;
            if (_playerControl.IsSkill)//skill damage x1.5 (...구조 개선 수정 필요)
                _finalAttackValue *= 1.5f;
            return _finalAttackValue;
        }

        public void UseSkill()//스킬사용 에너지 소모
        {
            SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.Energy) - 100f);
        }
        
        private IEnumerator RecoveryEnergy()//에너지회복
        {
            _isRecoverEnergy = true;
            yield return new WaitForSeconds(1f);
            SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.Energy) + 20f);
            if (GetStat(PlayerStatTypes.Energy) >= GetStat(PlayerStatTypes.MaxEnergy))
            {
                SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.MaxEnergy));
            }
            _isRecoverEnergy = false;
        }

        public void PlayerEquipWeapon(PlayerWeaponData data, GameObject prefab)
        {
            //WeaponData = data;//제거?
           
            Destroy(_equippedWeapon);
            GameObject newWeapon = prefab;
            _equippedWeapon = Instantiate(newWeapon, playerRightHand.transform);
            if (!data.IsDefaultWeapon)//기본무기가 아니면
            {
                _equippedWeapon.GetComponent<BoxCollider>().enabled = false; //드랍 아이템 체크용 BoxCollider 비활성
                _equippedWeapon.GetComponentInChildren<ParticleSystem>().Stop();//자식객체 ParticleSystem 이펙트 정지
            }
            
            if (data.AttackType == AttackType.Melee)//근접무기일시
            {
                //_equippedWeapon.AddComponent<PlayerMeleeAttack>();
            }
            _weaponAttackValue = data.AttackValue;
            SetStat(PlayerStatTypes.AttackValue, _weaponAttackValue);
            UpdateFinalAttackValue();
            _playerWeaponType = data.WeaponType;//WeaponType -> Animator change(애니메이터 추가 필요)
            if (data.GetEffects().Length != 0)
            {
                Debug.Log("have Effect");
            }
            else
            {
                Debug.Log("no Effect");
            }
        }

        public void PlayerEquipEquipment(PlayerEquipmentData data, GameObject prefab)
        {
            //EquipmentData = data;//제거?
            if (_equippedEquipment != null)
            {
                Destroy(_equippedEquipment);
            }
            
            if (data == null)
            {
                _equipmentDefenseValue = 0;
                SetStat(PlayerStatTypes.DefenseValue, 0);
                //itemEffects Remove
            }
            else
            {
                GameObject newEquipment = prefab;
                _equippedEquipment = Instantiate(newEquipment, playerHead.transform);
                _equippedEquipment.GetComponent<Collider>().enabled = false;
                _equippedEquipment.GetComponentInChildren<ParticleSystem>().Stop();
                _equipmentDefenseValue = data.DefenseValue;
                SetStat(PlayerStatTypes.DefenseValue, _equipmentDefenseValue);
                if (data.ItemEffect.Length != 0)
                {
                    Debug.Log("have Effect");
                }
                else
                {
                    Debug.Log("no Effect");
                }
            }
            
        }
        
        private void SetStat(PlayerStatTypes statType, float value)
        {
            if (_playerStats.ContainsKey(statType) && Mathf.Approximately(_playerStats[statType], value)) return;
            _playerStats[statType] = value;
            OnStatChanged?.Invoke(statType, value);
        }

        public float GetStat(PlayerStatTypes statType)
        {
            return _playerStats.GetValueOrDefault(statType, 0);
        }

        private void GetItem(GameObject item)
        {
            if (item.layer == (int)ItemLayers.Money)
            {
                Money money = item.GetComponent<Money>();
                OnGetMoney?.Invoke(money.moneyAmount);
                ObjectPoolingManager.Instance.ReturnToPool(PoolKeys.Money, item);
            }
            else
            {
                ItemDataAssign itemDataAssign = item.GetComponent<ItemDataAssign>();
                IItemData itemData = itemDataAssign.GetItemData();
                GameObject itemPrefab = itemData.GetItemPrefab();
                OnGetItem?.Invoke(new ItemDataWithID
                {
                    ItemData = itemData,
                    ItemPrefab = itemPrefab,
                    ItemType = itemData.ItemType,
                });
                if (item.layer == (int)ItemLayers.Chest)
                {
                    ObjectPoolingManager.Instance.ReturnToPool(PoolKeys.Chest01, item);
                }
                else
                {
                    Destroy(item);
                }
            }
        }

        public void DropItem(GameObject item)
        {
            Instantiate(item,transform.position + Vector3.back,Quaternion.identity);
        }
        
        public void UseItemQuickSlot(int slotNumber)
        {
            OnUseItemQuickSlot?.Invoke(slotNumber);
        }

        public void UseConsumableItem(ConsumableItemData data)
        {
            foreach (var itemEffect in data.ItemData)
            {
                if (itemEffect.effectType == EffectType.Instant)
                {
                    float effectAmount = itemEffect.effectAmount;
                    float currentValue = GetStat(itemEffect.effectStat);
                    switch (itemEffect.effectCalculate)
                    {
                        case CalculateType.Plus:
                            currentValue += effectAmount;
                            break;
                        case CalculateType.Multiply:
                            currentValue *= effectAmount;
                            break;
                        default:
                            break;
                    }
                    SetStat(itemEffect.effectStat, currentValue);
                }
                else
                {
                    //another effectType...
                    //Debug.Log("");
                }
                
                
            }
        }

        public void ActiveSwordAttackBox(bool isActive, bool isSkill)//공격Collider활성
        {
            swordAttackBox.SetActive(isActive);
            SetAttackEffect(isSkill);
        }

        private void SetAttackEffect(bool isSkill)
        {
            attackEffect.SetActive(!isSkill);
            skillEffect.SetActive(isSkill);
        }

        public override void Awake()
        {
            base.Awake();
            
            _playerControl = GetComponent<PlayerControl>();
            _mainCamera = Camera.main;
            _currentItemEffects = new List<ItemEffect>();
            _equipmentDefenseValue = 0f;
            //실제 들고있는 무기에... 무기교체 기능
            GameObject playerWeaponPrefab = playerJobData.DefaultWeapon;
            IItemData itemData = playerWeaponPrefab.GetComponent<ItemDataAssign>().GetItemData();
            
            if (itemData is PlayerWeaponData weaponData)
            {
                //WeaponData = weaponData;
                _weaponAttackValue = weaponData.AttackValue;//default weapon attack value
                _equippedWeapon = Instantiate(playerWeaponPrefab, playerRightHand.transform);
                PlayerDefaultWeaponData = weaponData;
            }
            else
            {
                Debug.Log("Why?");
            }
            //else...Ranged
           
            //PlayerJobData SO
            _playerStats = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, playerJobData.Health }, 
                { PlayerStatTypes.MaxHealth, playerJobData.MaxHealth },
                { PlayerStatTypes.Energy, playerJobData.Energy },
                { PlayerStatTypes.MaxEnergy, playerJobData.MaxEnergy },
                { PlayerStatTypes.AttackValue, _weaponAttackValue},
                { PlayerStatTypes.DefenseValue, 0f}
            };
            UpdateFinalAttackValue();//최종 공격력 갱신
            
        }

        private void Update()
        {
            if (GetStat(PlayerStatTypes.Energy) < GetStat(PlayerStatTypes.MaxEnergy) 
                && _isRecoverEnergy == false)
            {
                StartCoroutine(RecoveryEnergy());//에너지 회복
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("EnemyMeleeAttack") || other.CompareTag("EnemyRangedAttack")) 
                && _playerControl.IsDamaged == false && _playerControl.IsDodge == false)
            {
                float health = GetStat(PlayerStatTypes.Health);
                float damage = other.CompareTag("EnemyMeleeAttack") ?
                    other.GetComponent<EnemyMeleeAttack>().Damage : other.GetComponentInParent<EnemyProjectile>().Damage;
                health -= damage;
                SetStat(PlayerStatTypes.Health, health);
                Debug.Log("Player Health: "+health);
                StartCoroutine(Damaged(1f)); //피격 후 무적시간...1초
            }

            if (other.gameObject.layer == (int)ItemLayers.Money)
            {
                GetItem(other.gameObject);
            }
            
        }

        private void OnTriggerStay(Collider other)
        {
            //Distance로?(Vector...) 한번에 획득하지 못할때가 있음 - 수정필요
            if (other.CompareTag("Item") && other.gameObject.layer != (int)ItemLayers.Money)
            {
                if (other.gameObject.layer == (int)ItemLayers.Chest)
                {
                    OnFloatKey?.Invoke(FloatText.Open, other.transform.position);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        other.gameObject.GetComponent<Chest>().OpenChest(); 
                        OnExitFloatKey?.Invoke();
                    }
                       
                }
                else
                {
                    //Debug.Log(other.transform.position);
                    OnFloatKey?.Invoke(FloatText.Get, other.transform.position);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        GetItem(other.gameObject);
                        OnExitFloatKey?.Invoke();
                    }
                       
                   
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                if (other.gameObject.layer != (int)ItemLayers.Money)
                {
                    OnExitFloatKey?.Invoke();
                }
            }
        }

        private IEnumerator Damaged(float duration)
        {
            _playerControl.IsDamaged = true;
            hitEffect.SetActive(true);
            yield return new WaitForSeconds(duration);
            _playerControl.IsDamaged = false;
            hitEffect.SetActive(false);
        }
    }
}