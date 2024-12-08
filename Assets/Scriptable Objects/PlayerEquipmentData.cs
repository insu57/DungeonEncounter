using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerEquipmentData",
        menuName = "ScriptableObjects/PlayerEquipmentData", order = int.MaxValue)]
    public class PlayerEquipmentData : ScriptableObject
    {
        [SerializeField] private string equipmentName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private Rarity rarity;
        [SerializeField] private int defenseValue;
        [SerializeField] private ItemEffect[] itemEffect;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject itemPrefab;
        
        public string EquipmentName => equipmentName;
        public string Description => description;
        public string Type => type;
        public Rarity Rarity => rarity;
        public int DefenseValue => defenseValue;
        public ItemEffect[] ItemEffect => itemEffect;
        public Sprite Icon => icon;
        public GameObject ItemPrefab => itemPrefab;
    }
}
