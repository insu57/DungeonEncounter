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
        private GameObject _playerWeaponPrefab;//Weapon...Prefab
        [SerializeField]private GameObject playerRightHand;
        [SerializeField]private GameObject playerHead;
        private EnemyMeleeAttack _enemyMeleeAttack;
        private EnemyProjectile _enemyProjectile;
        public PlayerWeaponData WeaponData { get; private set; }
        private Dictionary<PlayerStatTypes, float> _playerStats; 
        public event Action<PlayerStatTypes,float> OnStatChanged;
        public float FinalDamage { get; private set; }
        
        private InventoryManager _playerInventoryManager;
        public event Action<int> OnGetMoney;
        public event Action<GameObject> OnGetItem;
        
        private Camera _mainCamera;


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
            }
            else
            {
                OnGetItem?.Invoke(item);
            }
            Destroy(item);
        }
        
        
        //무기,장비 변경
        //아이템 사용
        //사망 처리...리셋
        
        private void Awake()
        {
            
            _playerControl = GetComponent<PlayerControl>();
            _mainCamera = Camera.main;
            //실제 들고있는 무기에... 무기교체 기능
            _playerWeaponPrefab = playerJobData.DefaultWeapon;
            PlayerWeapon playerWeapon = _playerWeaponPrefab.GetComponent<PlayerWeapon>();
            WeaponData = playerWeapon.WeaponData;
            GameObject currentWeapon = Instantiate(_playerWeaponPrefab, playerRightHand.transform);
            //if(WeaponData.Type == )
            if (WeaponData.AttackType == AttackType.Melee)
            {
                currentWeapon.AddComponent<PlayerMeleeAttack>();
                currentWeapon.tag = "PlayerAttack";
            }
            //else...Ranged
           
            //직업 기본 데이터에서 받아오게 수정...SO에서 받아옴
            _playerStats = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, playerJobData.Health }, 
                { PlayerStatTypes.MaxHealth, playerJobData.MaxHealth },
                { PlayerStatTypes.Energy, playerJobData.Energy },
                { PlayerStatTypes.MaxEnergy, playerJobData.MaxEnergy },
                { PlayerStatTypes.AttackValue, WeaponData.AttackValue},
                { PlayerStatTypes.DefenseValue, 0f}
            };

            //_playerInventoryManager.CurrentWeaponData = _playerWeaponData;
            //_enemyProjectile = G
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