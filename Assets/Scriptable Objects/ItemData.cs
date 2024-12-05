using System;
using Player;
using UnityEngine;

namespace Scriptable_Objects
{
    
    
    [CreateAssetMenu(fileName = "ItemData",
        menuName = "ScriptableObjects/ItemData", order = int.MaxValue)]
    public class ItemData : ScriptableObject
    {
        //[SerializeField] 
        [SerializeField] private ItemTypes itemType;
        [SerializeField] private bool hasEffect;
        public ItemTypes ItemType => itemType;
        public bool HasEffect => hasEffect;
        
        [Header("공통 데이터")]
        [SerializeField] private string itemName;
        [SerializeField] private string itemDescription;
        [SerializeField] private Rarity rarity;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private ItemEffect[] itemEffect;
        public string ItemName => itemName;
        public string ItemDescription => itemDescription;
        public Rarity Rarity => rarity;
        public Sprite ItemIcon => itemIcon;
        public ItemEffect[] ItemEffect => itemEffect;
        
        [Header("무기 데이터")]
        [SerializeField] private float attackValue;
        [SerializeField] private WeaponType weaponType;
        public float AttackValue => attackValue;
        
        [Header("장비 데이터")]
        [SerializeField] private float defenseValue;
        public float DefenseValue => defenseValue;
   
        [Header("소비품 데이터")]
        [SerializeField] private ConsumableType consumableType;
        
    }
}