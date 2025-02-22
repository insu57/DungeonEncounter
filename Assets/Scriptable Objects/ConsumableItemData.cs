using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Player;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "ConsumableItemData",
        menuName = "ScriptableObjects/ConsumableItemData", order = int.MaxValue)]
    public class ConsumableItemData : ScriptableObject, IItemData
    {
        public class ConsumableData
        {
            public string itemName;
            public string description;
            public ConsumableType type;
            public Rarity rarity;
            public ItemEffect[] itemEffects;
        }

        [SerializeField] private TextAsset jsonFile;
        private ConsumableData data => JObject
            .Parse(Encoding.UTF8.GetString(jsonFile.bytes)).ToObject<ConsumableData>();
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        public ConsumableType Type => data.type;
        public string GetName()
        {
            return data.itemName;
        }

        public string GetDescription()
        {
            return data.description;
        }

        public Rarity GetRarity()
        {
            return data.rarity;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public ItemEffect[] GetEffects()
        {
            return data.itemEffects;
        }

        public GameObject GetItemPrefab()
        {
            return prefab;
        }
    }
}