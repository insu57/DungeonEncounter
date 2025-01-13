using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerWeaponData",
        menuName = "ScriptableObjects/PlayerWeaponData", order = int.MaxValue)]
    public class PlayerWeaponData : ScriptableObject, IItemData
    {
        [SerializeField] private string weaponName;
        [SerializeField] private string description;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private AttackType attackType;
        [SerializeField] private Rarity rarity;
        [SerializeField] private float attackValue;
        [SerializeField] private ItemEffect[] itemEffects;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool isDefaultWeapon;
        [SerializeField] private GameObject prefab;
        public string WeaponName => weaponName;
        public string Description => description;
        public WeaponType WeaponType => weaponType;
        public AttackType AttackType => attackType;
        public Rarity Rarity => rarity;
        public float AttackValue => attackValue;
        public ItemEffect[] ItemEffects => itemEffects;
        public Sprite Icon => icon;
        public bool IsDefaultWeapon => isDefaultWeapon;
        public ItemTypes ItemType => ItemTypes.Weapon;
        public string GetName()
        {
            return weaponName;
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
            return itemEffects;
        }

        public GameObject GetItemPrefab()
        {
            return prefab;
        }
    }
}