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
        [SerializeField] private PlayerJobData playerJobData;
        private PlayerControl _playerControl;
        [SerializeField] private GameObject playerRightHand;
        [SerializeField] private GameObject playerHead;
        private EnemyMeleeAttack _enemyMeleeAttack;

        private EnemyProjectile _enemyProjectile;

        //[SerializeField] private ItemPrefabData itemPrefabData;
        //public PlayerWeaponData WeaponData { get; private set; }
        //public PlayerEquipmentData EquipmentData { get; private set; }
        private Dictionary<PlayerStatTypes, float> _playerStats;
        public event Action<PlayerStatTypes, float> OnStatChanged;
        public event Action<PlayerStatTypes, bool> OnItemEffect;
        public PlayerWeaponData PlayerDefaultWeaponData { get; private set; }
        private GameObject _equippedWeapon;
        private float _weaponAttackValue;
        private WeaponType _playerWeaponType;
        private GameObject _equippedEquipment;
        private float _equipmentDefenseValue;
        private List<ItemEffect> _currentItemEffects; //다른 형태?(아이템별 효과 구분)
        private bool _isRecoverEnergy = false;
        private float _energyRecoverValue = 20f;
        private float _finalAttackValue;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject swordAttackBox;
        [SerializeField] private GameObject attackEffect;
        [SerializeField] private GameObject skillEffect;
        [SerializeField] private GameObject dodgeEffect;
        private readonly List<GameObject> _itemInRange = new List<GameObject>();
        public event Action<int> OnGetMoney;
        public event Action<ItemDataWithID> OnGetItem;
        public event Action<int> OnUseItemQuickSlot;
        public event Action<FloatText, Vector3> OnFloatKey;
        public event Action OnExitFloatKey;

        private Camera _mainCamera;

        public float GetFinalAttackValue()
        {
            _finalAttackValue = _weaponAttackValue;
            if (_playerControl.IsSkill) //skill damage x1.5 (...구조 개선 수정 필요)
                _finalAttackValue *= 1.5f;
            return _finalAttackValue;
        }

        private void TakeDamage(float damage)
        {
            var currentHealth = GetStat(PlayerStatTypes.Health);
            var defenseValue = GetStat(PlayerStatTypes.DefenseValue);
            currentHealth -= damage * (100f - defenseValue) / 100f;
            SetStat(PlayerStatTypes.Health, currentHealth);
        }

        public void UseSkill() //스킬사용 에너지 소모
        {
            SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.Energy) - 100f);
        }

        private IEnumerator RecoveryEnergy() //에너지회복
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
            if (!data.IsDefaultWeapon) //기본무기가 아니면
            {
                _equippedWeapon.GetComponent<BoxCollider>().enabled = false; //드랍 아이템 체크용 BoxCollider 비활성
                _equippedWeapon.GetComponentInChildren<ParticleSystem>().Stop(); //자식객체 ParticleSystem 이펙트 정지
            }

            if (data.AttackType == AttackType.Melee) //근접무기일시
            {
                //_equippedWeapon.AddComponent<PlayerMeleeAttack>();
            }

            _weaponAttackValue = data.AttackValue;
            SetStat(PlayerStatTypes.AttackValue, _weaponAttackValue);
            //UpdateFinalAttackValue();
            _playerWeaponType = data.WeaponType; //WeaponType -> Animator change(애니메이터 추가 필요)
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
                Destroy(item);
            }
        }

        public void DropItem(GameObject item)
        {
            Instantiate(item, transform.position + Vector3.back, Quaternion.identity);
        }

        public void GetItemInRange()
        {
            if(_itemInRange.Count == 0) return;

            for (var i = 0; i < _itemInRange.Count; i++)
            {
                if (_itemInRange[i].layer == (int)ItemLayers.Chest)
                {
                    _itemInRange[i].GetComponent<Chest>().OpenChest();
                }
                else
                {
                    GetItem(_itemInRange[i]); 
                }
                _itemInRange.RemoveAt(i);
            }
            
            OnExitFloatKey?.Invoke();
        }

         public void UseItemQuickSlot(int slotNumber)
        {
            OnUseItemQuickSlot?.Invoke(slotNumber);
        }

        public void UseConsumableItem(ConsumableItemData data)
        {
            foreach (var itemEffect in data.ItemData)
            {
                var calculateType = itemEffect.effectCalculate;
                var effectType = itemEffect.effectType;
                var statType = itemEffect.effectStat;
                var effectValue = itemEffect.effectValue;
                var currentValue = GetStat(itemEffect.effectStat);
                
                switch (effectType)
                {
                    case EffectType.Instant:
                    {
                        var maxValue = statType switch //최댓값
                        {
                            PlayerStatTypes.Health => GetStat(PlayerStatTypes.MaxHealth),
                            PlayerStatTypes.Energy => GetStat(PlayerStatTypes.MaxEnergy),
                            _ => currentValue
                        };

                        currentValue = statType switch
                        {
                            PlayerStatTypes.Health or PlayerStatTypes.Energy => Mathf.Clamp(
                                ItemEffectCalculate(calculateType, currentValue, effectValue), 0, maxValue),
                            
                            _ => ItemEffectCalculate(calculateType, currentValue, effectValue)
                        };
                        
                        SetStat(statType, currentValue);
                        break;
                    }
                    case EffectType.Temporary:
                        StartCoroutine(itemEffect.isTickBased ? 
                            TickBasedEffect(itemEffect) : TemporaryEffect(itemEffect));
                        break;
                    case EffectType.Permanent:
                        //SetStat(statType, ItemEffectCalculate(calculateType, currentValue, effectValue));
                        break;
                    default:
                        break;
                }
            }
        }

        private IEnumerator TickBasedEffect(ItemEffect itemEffect)
        {
            float startTime = Time.time;
            float currentValue = GetStat(itemEffect.effectStat);
            float maxValue = itemEffect.effectStat switch //최대값
            {
                PlayerStatTypes.Health => GetStat(PlayerStatTypes.MaxHealth),
                PlayerStatTypes.Energy => GetStat(PlayerStatTypes.MaxEnergy),
                _ => currentValue
            };
            
            while (Time.time - startTime < itemEffect.effectDuration)//Duration동안
            {
                currentValue = GetStat(itemEffect.effectStat);
                switch (itemEffect.effectStat)
                {
                    case PlayerStatTypes.Health:
                    case PlayerStatTypes.Energy:
                        if (currentValue < maxValue)
                        {
                            SetStat(PlayerStatTypes.Health, Mathf.Clamp(ItemEffectCalculate(itemEffect.effectCalculate, 
                                currentValue, itemEffect.effectValue), 0, maxValue));
                        }
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(itemEffect.tickSecond); //Tick마다 실행
            }
        }
        
        private IEnumerator TemporaryEffect(ItemEffect itemEffect)
        {
            float startTime = Time.time;
            float currentValue = GetStat(itemEffect.effectStat);
            
            SetStat(itemEffect.effectStat,
                ItemEffectCalculate(itemEffect.effectCalculate, currentValue, itemEffect.effectValue));
            //스탯 적용
            
            while (Time.time - startTime < itemEffect.effectDuration)
            {
                yield return null;
            }   
            //duration 지나면 원래대로
            SetStat(itemEffect.effectStat, currentValue); //적용중인 ItemEffect 관리-> Dictionary?
        }

        private static float ItemEffectCalculate(CalculateType calculateType, float currentValue, float effectValue)
        {
            return calculateType switch
            {
                CalculateType.Plus => currentValue + effectValue,
                CalculateType.Multiply => currentValue * effectValue,
                _ => currentValue
            };
        }
        
        
        public void ActiveSwordAttackBox(bool isActive, bool isSkill)//공격Collider활성
        {
            swordAttackBox.SetActive(isActive);
            SetAttackEffect(isSkill);
        }

        private void SetAttackEffect(bool isSkill)//공격 이펙트
        {
            attackEffect.SetActive(!isSkill);
            skillEffect.SetActive(isSkill);
        }

        public void ActiveDodgeEffect(bool isActive)
        {
            dodgeEffect.SetActive(isActive);
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
            //피격판정 처리
            if ((other.CompareTag("EnemyMeleeAttack") || other.CompareTag("EnemyRangedAttack")) 
                && _playerControl.IsDamaged == false && _playerControl.IsDodge == false)
            {
                var health = GetStat(PlayerStatTypes.Health);
                var damage = other.CompareTag("EnemyMeleeAttack") ?
                    other.GetComponent<EnemyMeleeAttack>().Damage 
                    : other.GetComponentInParent<EnemyProjectile>().Damage;
                TakeDamage(damage);
                //Debug.Log("Player Health: "+health);
                StartCoroutine(Damaged(1f)); //피격 후 무적시간...1초
            }

            //아이템 획득 처리
            if (other.CompareTag("Item"))
            {
                switch (other.gameObject.layer)
                {
                    case (int)ItemLayers.Money:
                        GetItem(other.gameObject);
                        break;
                    case (int)ItemLayers.Weapon or (int)ItemLayers.Equipment 
                        or (int)ItemLayers.Consumable:
                        OnFloatKey?.Invoke(FloatText.Get, other.transform.position);
                        _itemInRange.Add(other.gameObject);
                        Debug.Log("itemName "+ other.gameObject.name);
                        break;
                    case (int)ItemLayers.Chest:
                        OnFloatKey?.Invoke(FloatText.Open, other.transform.position+ new Vector3(0.3f,0,0));
                        _itemInRange.Add(other.gameObject);
                        // Debug.Log("itemInRange Count: "+_itemInRange.Count);
                        break;
                }
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                if (other.gameObject.layer != (int)ItemLayers.Money)
                {
                    _itemInRange.Remove(other.gameObject);
                    OnExitFloatKey?.Invoke();
                    //Debug.Log("itemInRange Count: "+_itemInRange.Count);
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