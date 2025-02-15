using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject dropPrefab;
        public int dropWeight;
    }

    [System.Serializable]
    public class RarityDropWeight
    {
        public Rarity rarity;
        public int dropWeight;
    }
    
    [CreateAssetMenu(fileName = "EnemyDropTable",
        menuName = "ScriptableObjects/EnemyDropTable", order = int.MaxValue)]
    public class EnemyDropTable : ScriptableObject
    {
        [Header("Money")]
        [SerializeField] private int moneyRangeStart;
        [SerializeField] private int moneyRangeEnd;
        [Header("Rarity Weight")]
        [SerializeField] private List<RarityDropWeight> rarityDropWeights;
        [Header("Chance")]
        [SerializeField] private float consumableChance;
        [SerializeField] private float chestChance;
        [SerializeField] private float weaponChance;
        [SerializeField] private float equipmentChance;

        public int MoneyRangeStart => moneyRangeStart;
        public int MoneyRangeEnd => moneyRangeEnd;
        public List<RarityDropWeight> RarityDropWeights => rarityDropWeights;
        public float ConsumableChance => consumableChance;
        public float ChestChance => chestChance;
        public float WeaponChance => weaponChance;
        public float EquipmentChance => equipmentChance;
    }
}