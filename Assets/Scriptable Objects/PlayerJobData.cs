using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerJobData",
        menuName = "ScriptableObjects/PlayerJobData", order = int.MaxValue)]
    public class PlayerJobData : ScriptableObject
    {
        [SerializeField] private string playerJobName;
        [SerializeField] private GameObject defaultWeapon;
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private float energy;
        [SerializeField] private float maxEnergy;
        [SerializeField] private float moveSpeed;
        //json에서 받아오게 수정 예정...
        
        public string PlayerJobName => playerJobName;
        public GameObject DefaultWeapon => defaultWeapon;
        public float Health => health;
        public float MaxHealth => maxHealth;
        public float Energy => energy;
        public float MaxEnergy => maxEnergy;
        public float MoveSpeed => moveSpeed;
    }
}
