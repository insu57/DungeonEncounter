using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerEquipmentData",
        menuName = "ScriptableObjects/PlayerEquipmentData", order = int.MaxValue)]
    public class PlayerEquipmentData : ScriptableObject, IItemData
    {
        [SerializeField] private string equipmentName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private Rarity rarity;
        [SerializeField] private int defenseValue;
        [SerializeField] private ItemEffect[] itemEffect;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        
        public string Type => type;
        public int DefenseValue => defenseValue;
        public ItemTypes ItemType => ItemTypes.Equipment;
        public string GetName()
        {
            return equipmentName;
        }

        public string GetDescription()
        {
            return description;
        }

        public Rarity GetRarity()
        {
            return rarity;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public ItemEffect[] GetEffects()
        {
            return itemEffect;
        }

        public GameObject GetItemPrefab()
        {
            return prefab;
        }
    }
}
