using System;
using System.Collections;
using System.Collections.Generic;
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
        private GameManager _gameManager;
        [SerializeField]private PlayerJobData playerJobData;
        private PlayerControl _playerControl;
        private GameObject _playerWeaponPrefab;//Weapon...Prefab
        public ItemData WeaponData { get; private set; }

        private Dictionary<PlayerStatTypes, float> _playerStats; 
        public event Action<PlayerStatTypes,float> OnStatChanged;
        
        private InventoryManager _playerInventoryManager;
        public event Action<float> OnGetMoney;
        
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
        
        
        
        //무기,장비 변경
        //아이템 사용
        //사망 처리...리셋
        
        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _playerControl = GetComponent<PlayerControl>();
            _mainCamera = Camera.main;
            
            _playerWeaponPrefab = playerJobData.DefaultWeapon;
            //PlayerWeapon playerWeapon = _playerWeaponPrefab.GetComponent<PlayerWeapon>();
            GetItemData playerWeapon = _playerWeaponPrefab.GetComponent<GetItemData>();
            WeaponData = playerWeapon.ItemData;
            
           
            //직업 기본 데이터에서 받아오게 수정...SO에서 받아옴
            
            Debug.Log(WeaponData.ItemName);
            Debug.Log(WeaponData.AttackValue);
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
        }
    
        private void Start() //구조 수정중
        {
            _playerWeaponPrefab.AddComponent<PlayerMeleeAttack>(); 
            //종료하고 추가됨... 개선필요
            //에디터 종료해도 안사라짐
        }

        private void OnApplicationQuit()
        {
            //Component 
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
            else if (other.CompareTag("Item"))
            {
                //Add to Inventory
                Debug.Log("Get Item!"+other.name);
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