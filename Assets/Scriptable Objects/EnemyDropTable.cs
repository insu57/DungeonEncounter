using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private float consumableChance;
        [SerializeField] private float chestChance;
        [SerializeField] private float weaponChance;
        [SerializeField] private float equipmentChance;
        //데이터 XML에서 받아오게 수정. Chest->등급별 리스트에서 가져오기
        public int MoneyRangeStart => moneyRangeStart;
        public int MoneyRangeEnd => moneyRangeEnd;
        public List<DropEntry> ConsumableItems => consumableItems;
        public List<DropEntry> Weapons => weapons;
        public List<DropEntry> Equipments => equipments;
        public float ConsumableChance => consumableChance;
        public float WeaponChance => weaponChance;
        public float EquipmentChance => equipmentChance;
    }
}