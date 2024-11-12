using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "EnemyData",
        menuName = "ScriptableObjects/EnemyData", order = int.MaxValue)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private string enemyName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private string rank;
        [SerializeField] private int maxHealth;
        [SerializeField] private float damage;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float attackFullFrame; //전체
        [SerializeField] private float attackStartFrame; //공격 판정 시작
        [SerializeField] private float attackEndFrame; //공격 판정 끝
        
        public string EnemyName => enemyName;
        public string Description => description;
        public string Type => type;
        public string Rank => rank;
        public float MaxHealth => maxHealth;
        public float Damage => damage;
        public GameObject ProjectilePrefab => projectilePrefab;
        public float ProjectileSpeed => projectileSpeed;
        public float AttackFullFrame => attackFullFrame;
        public float AttackStartFrame => attackStartFrame;
        public float AttackEndFrame => attackEndFrame;
    }
}
