using System.Collections.Generic;
using UnityEngine;

namespace Scriptable_Objects
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject dropPrefab;
        public int dropChance;
    }

    [CreateAssetMenu(fileName = "EnemyDropTable",
        menuName = "ScriptableObjects/EnemyDropTable", order = int.MaxValue)]
    public class EnemyDropTable : ScriptableObject
    { 
        [SerializeField] private int moneyRangeStart;
        [SerializeField] private int moneyRangeEnd;
        [SerializeField] private List<DropEntry> consumableItems;
        [SerializeField] private List<DropEntry> weapons;
        [SerializeField] private List<DropEntry> equipments;
        [SerializeField] private float itemChance;
        [SerializeField] private float weaponChance;
        [SerializeField] private float equipmentChance;
    
        public int MoneyRangeStart => moneyRangeStart;
        public int MoneyRangeEnd => moneyRangeEnd;
        public List<DropEntry> ConsumableItems => consumableItems;
        public List<DropEntry> Weapons => weapons;
        public List<DropEntry> Equipments => equipments;
        public float ItemChance => itemChance;
        public float WeaponChance => weaponChance;
        public float EquipmentChance => equipmentChance;
    }
}