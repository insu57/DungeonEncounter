using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Scriptable_Objects;
using UnityEngine;
namespace Player
{
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
        
        private PlayerWeaponData _equippedWeaponData;
        private PlayerEquipmentData _equippedEquipmentData;
        private Dictionary<PlayerStatTypes, float> _playerStats;
        public event Action<PlayerStatTypes, float> OnStatChanged;
        public event Action<PlayerStatTypes, float> OnTemporaryItemEffect;//UI
        public PlayerWeaponData PlayerDefaultWeaponData { get; private set; }
        private GameObject _equippedWeapon;
        private WeaponType _playerWeaponType;
        private GameObject _equippedEquipment;
        
        private Dictionary<PlayerStatTypes, float> _currentPlusItemEffects;
        private Dictionary<PlayerStatTypes, float> _currentMultiplyItemEffects;
        private readonly Dictionary<ItemEffect, Coroutine> _currentActiveItemEffect = new Dictionary<ItemEffect, Coroutine>();
        
        private bool _isRecoverEnergy = false;
        private float _energyRecoverValue = 20f; //에너지 회복량
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
            _finalAttackValue = GetFinalStat(PlayerStatTypes.AttackValue);
            if (_playerControl.IsSkill) //skill damage x1.5 (...구조 개선 수정 필요)
                _finalAttackValue *= 1.5f;
            return _finalAttackValue;
        }

        private void TakeDamage(float damage)
        {
            var currentHealth = GetStat(PlayerStatTypes.Health);
            var defenseValue = GetFinalStat(PlayerStatTypes.DefenseValue);
            currentHealth -= damage * (100f - defenseValue) / 100f;
            SetStat(PlayerStatTypes.Health, currentHealth);
        }

        public float GetFinalStat(PlayerStatTypes stat)
        {
            var currentValue = GetStat(stat);
            var plusValue = _currentPlusItemEffects[stat];
            var multiplyValue = _currentMultiplyItemEffects[stat];
            return (currentValue + plusValue) * multiplyValue;
        }
        
        private void SetFinalStat(PlayerStatTypes stat)
        {
            var currentValue = GetStat(stat);
            var plusValue = _currentPlusItemEffects[stat];
            var multiplyValue = _currentMultiplyItemEffects[stat];
            OnStatChanged?.Invoke(stat, (currentValue + plusValue) * multiplyValue);
        }
        
        public void UseSkill() //스킬사용 에너지 소모
        {
            SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.Energy) - 100f);
        }

        private IEnumerator RecoveryEnergy() //에너지회복
        {
            _isRecoverEnergy = true;
            yield return new WaitForSeconds(1f);
            SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.Energy) + _energyRecoverValue);
            if (GetStat(PlayerStatTypes.Energy) >= GetStat(PlayerStatTypes.MaxEnergy))
            {
                SetStat(PlayerStatTypes.Energy, GetStat(PlayerStatTypes.MaxEnergy));
            }

            _isRecoverEnergy = false;
        }

        public void PlayerEquipWeapon(PlayerWeaponData data, GameObject prefab)
        {
            if (_equippedWeaponData != null && _equippedWeaponData.GetEffects().Length != 0)
                //기존 아이템이 있을때 아이템 효과가 있으면 
            {
                foreach (var itemEffect in _equippedWeaponData.GetEffects()) //기존 효과 정지
                {
                    if (itemEffect.effectType != EffectType.Permanent) continue;
                    
                    StopCoroutine(_currentActiveItemEffect[itemEffect]);
                    if (!itemEffect.isTickBased)
                    {
                        ItemEffectCalculate(itemEffect.effectCalculate, itemEffect.effectStat,
                            itemEffect.effectValue, true);
                    }
                    _currentActiveItemEffect.Remove(itemEffect);
                }
            }
            
            if (_equippedWeaponData == null) //없으면 할당
            {
                _equippedWeaponData = data;
            }
            
            foreach (var itemEffect in data.GetEffects())
            {
                if (itemEffect.effectType == EffectType.Permanent)
                {
                    var coroutine =  StartCoroutine(itemEffect.isTickBased ? 
                        TickPersistantEffect(itemEffect) : NonTickPersistantEffect(itemEffect));
                    _currentActiveItemEffect[itemEffect] = coroutine; //아이템 효과 코루틴 관리
                }
                else
                {
                    Debug.LogWarning("Weapon Item Effect Type is not Permanent");
                }
            }
            
            SetStat(PlayerStatTypes.AttackValue, data.AttackValue);
            
            Destroy(_equippedWeapon);
            _equippedWeapon = Instantiate(prefab, playerRightHand.transform);
            
            if (!data.IsDefaultWeapon) //기본무기가 아니면
            {
                _equippedWeapon.GetComponent<BoxCollider>().enabled = false; //드랍 아이템 체크용 BoxCollider 비활성
                _equippedWeapon.GetComponentInChildren<ParticleSystem>().Stop(); //자식객체 ParticleSystem 이펙트 정지
            }

            if (data.AttackType == AttackType.Melee) //근접무기일시
            {
                //_equippedWeapon.AddComponent<PlayerMeleeAttack>();
            }
            
            _playerWeaponType = data.WeaponType; //WeaponType -> Animator change(애니메이터 추가 필요)
            
        }

        public void PlayerEquipEquipment(PlayerEquipmentData data, GameObject prefab)
        {
            if (_equippedEquipment != null)
            {
                Destroy(_equippedEquipment);
            }

            if (data == null)
            {
                SetStat(PlayerStatTypes.DefenseValue, 0);
                //itemEffects Remove
                
                foreach (var itemEffect in _equippedEquipmentData.GetEffects())
                {
                    if (itemEffect.effectType != EffectType.Permanent) continue;
                    //Debug.Log("Data null...Unequip...Item Effect Remove...StopCoroutine");
                    StopCoroutine(_currentActiveItemEffect[itemEffect]);
                    if (!itemEffect.isTickBased)
                    {
                        //Debug.Log("Unequip...Item Effect Remove");
                        ItemEffectCalculate(itemEffect.effectCalculate, itemEffect.effectStat,
                            itemEffect.effectValue, true);
                    }
                }
                _equippedEquipmentData = null;
            }
            else
            {
                
                if (_equippedEquipmentData != null)
                {
                    foreach (var itemEffect in _equippedEquipmentData.GetEffects())
                    {
                        if (itemEffect.effectType != EffectType.Permanent) continue;
                        //Debug.Log("EquipmentChange...ItemEffect Remove...StopCoroutine");
                        StopCoroutine(_currentActiveItemEffect[itemEffect]);
                        if (!itemEffect.isTickBased)
                        {
                            //Debug.Log("Equipment Change...Item Effect Remove");
                            ItemEffectCalculate(itemEffect.effectCalculate, itemEffect.effectStat,
                                itemEffect.effectValue, true);
                        }
                    }
                }
                
                _equippedEquipmentData = data;
                
                foreach (var itemEffect in data.GetEffects())
                {
                    if (itemEffect.effectType == EffectType.Permanent)
                    {
                        //Debug.Log("Equip...Have Item Effect...Start Coroutine");
                        var coroutine =  StartCoroutine(itemEffect.isTickBased ? 
                            TickPersistantEffect(itemEffect) : NonTickPersistantEffect(itemEffect));
                        _currentActiveItemEffect[itemEffect] = coroutine; //아이템 효과 코루틴 관리
                    }
                    else
                    {
                        //Debug.LogWarning("Equipment Item Effect Type is not Permanent");
                    }
                }
                
                _equippedEquipment = Instantiate(prefab, playerHead.transform);
                _equippedEquipment.GetComponent<Collider>().enabled = false;
                _equippedEquipment.GetComponentInChildren<ParticleSystem>().Stop();
                SetStat(PlayerStatTypes.DefenseValue,data.DefenseValue);
            }
        }

        private void SetStat(PlayerStatTypes statType, float value)
        {
            if (_playerStats.ContainsKey(statType) && Mathf.Approximately(_playerStats[statType], value)) return;
            _playerStats[statType] = value;
            var statValue = value;
            var plusValue = _currentPlusItemEffects[statType];
            var multiplyValue = _currentMultiplyItemEffects[statType];
            OnStatChanged?.Invoke(statType, (value + plusValue) * multiplyValue);
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
            foreach (var itemEffect in data.ItemData)//소비템의 아이템 효과 데이터
            {
                var effectType = itemEffect.effectType;
                var statType = itemEffect.effectStat;
                var effectValue = itemEffect.effectValue;
                var currentValue = GetStat(itemEffect.effectStat);
                
                switch (effectType)
                {
                    case EffectType.Instant: //즉시 발동 효과 - 일단은 체력/에너지만
                    {
                        var maxValue = statType switch //최댓값
                        {
                            PlayerStatTypes.Health => GetFinalStat(PlayerStatTypes.MaxHealth),
                            PlayerStatTypes.Energy => GetFinalStat(PlayerStatTypes.MaxEnergy),
                            _ => currentValue
                        };

                        if (statType is PlayerStatTypes.Health or PlayerStatTypes.Energy)
                        {
                            currentValue = itemEffect.effectCalculate switch
                            {
                               CalculateType.Plus => Mathf.Clamp(currentValue + effectValue, 0, maxValue),
                               CalculateType.Multiply => Mathf.Clamp(currentValue * effectValue, 0, maxValue),
                               _ => currentValue
                            };
                        }
                        else
                        {
                            currentValue = itemEffect.effectCalculate switch
                            {
                                CalculateType.Plus => currentValue + effectValue,
                                CalculateType.Multiply => currentValue * effectValue,
                                _ => currentValue
                            };
                        }
                        SetStat(statType, currentValue);
                        break;
                    }
                    //지속효과.
                    case EffectType.Temporary: 
                        StartCoroutine(itemEffect.isTickBased ? //틱당 효과
                            TickPersistantEffect(itemEffect) : NonTickPersistantEffect(itemEffect));
                        break;
                    case EffectType.Permanent:
                       var coroutine = StartCoroutine(itemEffect.isTickBased ? //틱당 효과
                            TickPersistantEffect(itemEffect) : NonTickPersistantEffect(itemEffect));
                        _currentActiveItemEffect[itemEffect] = coroutine;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IEnumerator TickPersistantEffect(ItemEffect itemEffect)
        {
            //Tick만큼 발동 -> 체력/에너지 스탯 (일단 다른 스탯은 고려x)
            var startTime = Time.time; //시작시간
            var currentValue = GetStat(itemEffect.effectStat);//현재값
            var maxValue = itemEffect.effectStat switch //최댓값
            {
                PlayerStatTypes.Health => GetStat(PlayerStatTypes.MaxHealth),
                PlayerStatTypes.Energy => GetStat(PlayerStatTypes.MaxEnergy),
                _ => currentValue
            };

            var isPermanent = itemEffect.effectType == EffectType.Permanent;
            
            while (isPermanent || Time.time - startTime < itemEffect.effectDuration)//Duration동안 or 영구지속이면
            {
                currentValue = GetStat(itemEffect.effectStat);

                if (itemEffect.effectStat is PlayerStatTypes.Health or PlayerStatTypes.Energy)
                {
                    if (currentValue < maxValue)
                    {
                        currentValue = itemEffect.effectCalculate switch
                        {
                            CalculateType.Plus => Mathf.Clamp(currentValue + itemEffect.effectValue, 0,
                                maxValue),
                            CalculateType.Multiply => Mathf.Clamp(currentValue * itemEffect.effectValue, 0,
                                maxValue),
                            _ => currentValue
                        };
                        SetStat(itemEffect.effectStat, currentValue);
                    }
                }
                yield return new WaitForSeconds(itemEffect.tickSecond); //Tick마다 실행
            }
            //일시지속이면 Duration동안 실행. 영구지속이면 StopCoroutine으로 정지해야함.
        }
        
        private IEnumerator NonTickPersistantEffect(ItemEffect itemEffect)
        {
            float startTime = Time.time;
            ItemEffectCalculate(itemEffect.effectCalculate, itemEffect.effectStat, itemEffect.effectValue, false);
            //스탯 적용
            
            bool isPermanent = itemEffect.effectType == EffectType.Permanent;

            while (isPermanent || Time.time - startTime < itemEffect.effectDuration)
            {
                yield return null;
            } 
            //duration 지나면 원래대로 , 영구지속일 경우 따로 아래 코드 실행 필요.
            ItemEffectCalculate(itemEffect.effectCalculate, itemEffect.effectStat, itemEffect.effectValue, true);
        }

        private void ItemEffectCalculate(CalculateType calculateType, PlayerStatTypes stat, float value, bool isRemove) 
        {
            Debug.Log("before: "+_currentPlusItemEffects[stat]);
            if (calculateType == CalculateType.Plus)
            {
                if (isRemove)
                {
                    _currentPlusItemEffects[stat] -= value;
                }
                else
                {
                    _currentPlusItemEffects[stat] += value;
                }
            }
            else
            {
                if (isRemove)
                {
                    _currentMultiplyItemEffects[stat] /= value;
                }
                else
                {
                    _currentMultiplyItemEffects[stat] *= value;
                }
               
            }
            Debug.Log("after: "+_currentPlusItemEffects[stat]);
            SetFinalStat(stat);
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
            
            //_equipmentDefenseValue = 0f;
            //실제 들고있는 무기에... 무기교체 기능
            GameObject playerWeaponPrefab = playerJobData.DefaultWeapon;
            IItemData itemData = playerWeaponPrefab.GetComponent<ItemDataAssign>().GetItemData();

            var attackValue = 0f;
            if (itemData is PlayerWeaponData weaponData)
            {
                //WeaponData = weaponData;
                //_weaponAttackValue = weaponData.AttackValue;//default weapon attack value
                attackValue = weaponData.AttackValue;
                _equippedWeapon = Instantiate(playerWeaponPrefab, playerRightHand.transform);
                PlayerDefaultWeaponData = weaponData;
            }
            //else...Ranged
           
            //PlayerJobData SO
            _playerStats = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, playerJobData.Health }, 
                { PlayerStatTypes.MaxHealth, playerJobData.MaxHealth },
                { PlayerStatTypes.Energy, playerJobData.Energy },
                { PlayerStatTypes.MaxEnergy, playerJobData.MaxEnergy },
                { PlayerStatTypes.AttackValue, attackValue},
                { PlayerStatTypes.DefenseValue, 0f}
            };

            _currentPlusItemEffects = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, 0f },
                { PlayerStatTypes.MaxHealth, 0f },
                { PlayerStatTypes.Energy, 0f },
                { PlayerStatTypes.MaxEnergy, 0f},
                { PlayerStatTypes.AttackValue , 0f},
                { PlayerStatTypes.DefenseValue, 0f}
            };
            _currentMultiplyItemEffects = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, 1f },
                { PlayerStatTypes.MaxHealth, 1f },
                { PlayerStatTypes.Energy, 1f },
                { PlayerStatTypes.MaxEnergy, 1f },
                { PlayerStatTypes.AttackValue, 1f },
                { PlayerStatTypes.DefenseValue, 1f },
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
                //var health = GetStat(PlayerStatTypes.Health);
                var damage = other.CompareTag("EnemyMeleeAttack") ?
                    other.GetComponent<EnemyMeleeAttack>().Damage 
                    : other.GetComponentInParent<EnemyProjectile>().Damage;
                TakeDamage(damage);
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
                        break;
                    case (int)ItemLayers.Chest:
                        OnFloatKey?.Invoke(FloatText.Open, other.transform.position+ new Vector3(0.3f,0,0));
                        _itemInRange.Add(other.gameObject);
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