using System;
using Player;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "ConsumableItemData",
        menuName = "ScriptableObjects/ConsumableItemData", order = int.MaxValue)]
    public class ConsumableItemData : ScriptableObject, IItemData
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private ConsumableType type;
        [SerializeField] private Rarity rarity;
        [SerializeField] private ItemEffect[] itemData;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        public ConsumableType Type => type;
        public ItemTypes ItemType => ItemTypes.Consumable;
        public string GetName()
        {
            return itemName;
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
            return itemData;
        }

        public GameObject GetItemPrefab()
        {
            return prefab;
        }
    }
}