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
        [SerializeField] private string rarity;
        [SerializeField] private Sprite icon;

        public string EquipmentName => equipmentName;
        public string Description => description;
        public string Type => type;
        public string Rarity => rarity;
        public Sprite Icon => icon;
    }
}
