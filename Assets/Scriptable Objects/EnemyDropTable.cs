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

    [CreateAssetMenu(fileName = "EnemyDropTable",
        menuName = "ScriptableObjects/EnemyDropTable", order = int.MaxValue)]
    public class EnemyDropTable : ScriptableObject
    {
        [Header("Money")]
        [SerializeField] private GameObject moneyPrefab;
        [SerializeField] private int moneyRangeStart;
        [SerializeField] private int moneyRangeEnd;
        [Header("Consumables")]
        [SerializeField] private List<DropEntry> consumableItems;
        [SerializeField] private int consumableTotalWeight;
        [Header("Weapons")]
        [SerializeField] private List<DropEntry> weapons;
        [SerializeField] private int weaponsTotalWeight;
        [Header("Equipments")]
        [SerializeField] private List<DropEntry> equipments;
        [SerializeField] private int equipmentTotalWeight;
        [Header("Chance")]
        [SerializeField] private float consumableChance;
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private float chestChance;
        [SerializeField] private float weaponChance;
        [SerializeField] private float equipmentChance;
        //데이터 XML에서 받아오게 수정. Chest->등급별 리스트에서 가져오기
        public GameObject MoneyPrefab => moneyPrefab;
        public int MoneyRangeStart => moneyRangeStart;
        public int MoneyRangeEnd => moneyRangeEnd;
        public List<DropEntry> ConsumableItems => consumableItems;
        public int ConsumableTotalWeight => consumableTotalWeight;
        public List<DropEntry> Weapons => weapons;
        public int WeaponsTotalWeight => weaponsTotalWeight;
        public List<DropEntry> Equipments => equipments;
        public int EquipmentTotalWeight => equipmentTotalWeight;
        public float ConsumableChance => consumableChance;
        public GameObject ChestPrefab => chestPrefab;
        public float ChestChance => chestChance;
        public float WeaponChance => weaponChance;
        public float EquipmentChance => equipmentChance;
    }
}