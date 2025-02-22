using System;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerWeaponData",
        menuName = "ScriptableObjects/PlayerWeaponData", order = int.MaxValue)]
    public class PlayerWeaponData : ScriptableObject, IItemData
    {
        private class WeaponData
        {
            public string itemName;
            public string description;
            public WeaponType weaponType;
            public AttackType attackType;
            public Rarity rarity;
            public float attackValue;
            public ItemEffect[] itemEffects;
            public bool isDefaultWeapon;
        }
        
        [SerializeField] private TextAsset jsonFile;
        private WeaponData data => JObject
            .Parse(Encoding.UTF8.GetString(jsonFile.bytes)).ToObject<WeaponData>();
        private WeaponType weaponType => data.weaponType;
        private AttackType attackType => data.attackType;
        private float attackValue => data.attackValue;
        private bool isDefaultWeapon => data.isDefaultWeapon;
        
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;
        
        public WeaponType WeaponType => weaponType;
        public AttackType AttackType => attackType;
        public float AttackValue => attackValue;
        public bool IsDefaultWeapon => isDefaultWeapon;
        public ItemTypes ItemType => ItemTypes.Weapon;
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