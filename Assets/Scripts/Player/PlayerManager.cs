using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Scriptable_Objects;
using Unity.VisualScripting;
using UnityEngine;
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

    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]private PlayerJobData playerJobData;
        private PlayerControl _playerControl;
        [SerializeField]private GameObject playerRightHand;
        [SerializeField]private GameObject playerHead;
        private EnemyMeleeAttack _enemyMeleeAttack;
        private EnemyProjectile _enemyProjectile;
        [SerializeField] private ItemPrefabData itemPrefabData;
        public PlayerWeaponData WeaponData { get; private set; }
        public PlayerEquipmentData EquipmentData { get; private set; }
        private Dictionary<PlayerStatTypes, float> _playerStats; 
        public event Action<PlayerStatTypes,float> OnStatChanged;
        private GameObject _equippedWeapon;
        private float _weaponAttackValue;
        private WeaponType _playerWeaponType;
        private GameObject _equippedEquipment;
        private float _equipmentDefenseValue;
        private List<ItemEffect> _currentItemEffects;//다른 형태?(아이템별 효과 구분)
        public float FinalAttackValue { get; private set; }
        
        public event Action<int> OnGetMoney;
        public event Action<GameObject> OnGetItem;
        public event Action<int> OnUseItemQuickSlot;
        
        private Camera _mainCamera;
        public void UpdateFinalAttackValue()//각종 효과 구현 진행 필요.
        {
            FinalAttackValue = _weaponAttackValue;
            //+itemEffect...
        }

        public void PlayerEquipWeapon(PlayerWeaponData data)
        {
            WeaponData = data;//제거?
           
            Destroy(_equippedWeapon);
            GameObject newWeapon = itemPrefabData.GetWeaponPrefab(data);
            _equippedWeapon = Instantiate(newWeapon, playerRightHand.transform);
            if (data.AttackType == AttackType.Melee)
            {
                _equippedWeapon.AddComponent<PlayerMeleeAttack>();
                _equippedWeapon.tag = "PlayerAttack";
            }
            _weaponAttackValue = data.AttackValue;
            SetStat(PlayerStatTypes.AttackValue, _weaponAttackValue);
            UpdateFinalAttackValue();
            _playerWeaponType = data.WeaponType;//WeaponType -> Animator change(애니메이터 추가 필요)
            if (data.ItemEffects.Length != 0)
            {
                Debug.Log("have Effect");
            }
            else
            {
                Debug.Log("no Effect");
            }
        }

        public void PlayerEquipEquipment(PlayerEquipmentData data)
        {
            
            EquipmentData = data;//제거?
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
                GameObject newEquipment = itemPrefabData.GetEquipmentPrefab(data);
                _equippedEquipment = Instantiate(newEquipment, playerHead.transform);
                _equippedEquipment.GetComponent<Collider>().enabled = false;
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
                OnGetItem?.Invoke(item);
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
        
        private void Awake()
        {
            
            _playerControl = GetComponent<PlayerControl>();
            _mainCamera = Camera.main;
            _currentItemEffects = new List<ItemEffect>();
            _equipmentDefenseValue = 0f;
            //실제 들고있는 무기에... 무기교체 기능
            GameObject playerWeaponPrefab = playerJobData.DefaultWeapon;
            PlayerWeapon playerWeapon = playerWeaponPrefab.GetComponent<PlayerWeapon>();
            WeaponData = playerWeapon.WeaponData;
            _weaponAttackValue = WeaponData.AttackValue;//default weapon attack value
            _equippedWeapon = Instantiate(playerWeaponPrefab, playerRightHand.transform);
            //if(WeaponData.Type == )
            if (WeaponData.AttackType == AttackType.Melee)
            {
                _equippedWeapon.AddComponent<PlayerMeleeAttack>();
                _equippedWeapon.tag = "PlayerAttack";
            }
            //else...Ranged
           
            //PlayerJobData SO
            _playerStats = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, playerJobData.Health }, 
                { PlayerStatTypes.MaxHealth, playerJobData.MaxHealth },
                { PlayerStatTypes.Energy, playerJobData.Energy },
                { PlayerStatTypes.MaxEnergy, playerJobData.MaxEnergy },
                { PlayerStatTypes.AttackValue, WeaponData.AttackValue},
                { PlayerStatTypes.DefenseValue, 0f}
            };
            UpdateFinalAttackValue();//최종 공격력 갱신
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
            
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                if (other.gameObject.layer == (int)ItemLayers.Chest)
                {
                    if(Input.GetKeyDown(KeyCode.F))
                        other.gameObject.GetComponent<Chest>().OpenChest(); //Float Image OPEN KEY 'F'
                }
                else if(other.gameObject.layer == (int)ItemLayers.Money)
                {
                    GetItem(other.gameObject);
                }
                else
                {
                    if(Input.GetKeyDown(KeyCode.F))
                        GetItem(other.gameObject);
                }
            }
        }

        private IEnumerator Damaged(float duration)
        {
            _playerControl.IsDamaged = true;
            yield return new WaitForSeconds(duration);
            _playerControl.IsDamaged = false;
        }
    }
}