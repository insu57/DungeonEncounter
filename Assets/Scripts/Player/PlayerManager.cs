using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
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
        private GameObject _playerWeaponPrefab;
        private PlayerWeapon _playerWeapon;
        private PlayerWeaponData _playerWeaponData;
        
        private Dictionary<PlayerStatTypes, float> _playerStats;
        public event Action<PlayerStatTypes,float> OnStatChanged;
        
        private Camera _mainCamera;
        

        public void SetStat(PlayerStatTypes statType, float value)
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
            _playerWeapon = _playerWeaponPrefab.GetComponent<PlayerWeapon>();
            _playerWeaponData = _playerWeapon.Data;
            //직업 기본 데이터에서 받아오게 수정...SO에서 받아옴
            
            _playerStats = new Dictionary<PlayerStatTypes, float>
            {
                { PlayerStatTypes.Health, playerJobData.Health }, 
                { PlayerStatTypes.MaxHealth, playerJobData.MaxHealth },
                { PlayerStatTypes.Energy, playerJobData.Energy },
                { PlayerStatTypes.MaxEnergy, playerJobData.MaxEnergy },
                { PlayerStatTypes.AttackValue, _playerWeaponData.AttackValue},
                { PlayerStatTypes.DefenseValue, 0f}
            };
            
        }
    
        private void Start() //구조 수정중
        {
            //Init처리 필요...초기값은 Invoke되지않은듯...
            SetStat(PlayerStatTypes.Health, GetStat(PlayerStatTypes.Health) );
            Debug.Log(_playerStats[PlayerStatTypes.Health]);
            SetStat(PlayerStatTypes.Energy, _playerStats[PlayerStatTypes.Energy] );
            SetStat(PlayerStatTypes.AttackValue, _playerStats[PlayerStatTypes.AttackValue]);
            SetStat(PlayerStatTypes.DefenseValue, _playerStats[PlayerStatTypes.DefenseValue]);
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