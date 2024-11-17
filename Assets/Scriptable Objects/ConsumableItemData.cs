using UnityEngine;

namespace Scriptable_Objects
{
    [System.Serializable]
    public class ItemEffect
    {
        public string effectType;
        public string effectCalc;
        public string effect;
    }

    [CreateAssetMenu(fileName = "ConsumableItemData",
        menuName = "ScriptableObjects/ConsumableItemData", order = int.MaxValue)]
    public class ConsumableItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private string rarity;
        [SerializeField] private ItemEffect[] itemData;

    }
}