using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "EnemyData",
        menuName = "Scriptable Objects/EnemyData", order = int.MaxValue)]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private string enemyName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private string rank;
        [SerializeField] private int maxHealth;
        [SerializeField] private float damage;

        public string EnemyName => enemyName;
        public string Description => description;
        public string Type => type;
        public string Rank => rank;
        public float MaxHealth => maxHealth;
        public float Damage => damage;
    
    }
}
