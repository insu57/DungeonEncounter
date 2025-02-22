using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerEquipmentData",
        menuName = "ScriptableObjects/PlayerEquipmentData", order = int.MaxValue)]
    public class PlayerEquipmentData : ScriptableObject, IItemData
    {
        private class EquipmentData
        {
            public string itemName;
            public string description;
            public string type;
            public Rarity rarity;
            public float defenseValue;
            public ItemEffect[] itemEffects;
        }
        
        [SerializeField] private TextAsset jsonFile;
        private EquipmentData data => JObject
            .Parse(Encoding.UTF8.GetString(jsonFile.bytes)).ToObject<EquipmentData>();
        
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        
        public string Type => data.type;
        public float DefenseValue => data.defenseValue;
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
