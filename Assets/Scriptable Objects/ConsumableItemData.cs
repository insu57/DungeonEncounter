using Player;
using UnityEngine;

namespace Scriptable_Objects
{
    [System.Serializable]
    public class ItemEffect
    {
        public PlayerStatTypes effectStat;
        public CalculateType effectCalc;
        public string effect;
        //즉발.지연 추가 필요
    }
    
    [CreateAssetMenu(fileName = "ConsumableItemData",
        menuName = "ScriptableObjects/ConsumableItemData", order = int.MaxValue)]
    public class ConsumableItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private ConsumableType type;
        [SerializeField] private Rarity rarity;
        [SerializeField] private ItemEffect[] itemData;
        [SerializeField] private Sprite icon;
        public string ItemName => itemName;
        public string Description => description;
        public ConsumableType Type => type;
        public Rarity Rarity => rarity;
        public ItemEffect[] ItemData => itemData;
        public Sprite Icon => icon;
    }
}