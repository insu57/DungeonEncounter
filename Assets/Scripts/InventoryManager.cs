using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    public class ConsumableItem
    {
        public ConsumableItemData ItemData;
        public int Quantity;
    }
    
    private PlayerManager _playerManager;
    [SerializeField] private ItemPrefabData itemPrefabData;
    private GameObject _currentEquipment;
    private GameObject _itemQuickSlot1;
    private GameObject _itemQuickSlot2;
    
    private int _moneyAmount;
    public PlayerWeaponData CurrentWeaponData { private set; get; }
    public PlayerEquipmentData CurrentEquipmentData { private set; get; }
    public ConsumableItemData ItemQuickSlot1 { private set; get; }
    public ConsumableItemData ItemQuickSlot2 { private set; get; }

    private GameObject _selectedItem; //GameObject?Data?...
    public List<PlayerWeaponData> WeaponDataList { get; } = new List<PlayerWeaponData>();
    public List<PlayerEquipmentData> EquipmentDataList { get; } = new List<PlayerEquipmentData>();
    public List<ConsumableItem> ConsumableDataList { get; } = new List<ConsumableItem>();

    //public Action 
    //public Event changeWeapon;
    //public Event changeEquipment;
    public event Action<int> UpdateMoneyAmount; 
    public Event UpdateInventory;

    public int weaponInventoryCount { private set; get; }
    public int weaponInventoryMaxCount { private set; get; }

    public int equipmentInventoryCount { private set; get; }
    public int equipmentInventoryMaxCount { private set; get; }

    public int consumableInventoryCount { private set; get; }
    public int consumableInventoryMaxCount { private set; get; }
    //아이템별 현재 수량...
    public int consumableItemMaxQuantity { private set; get; }
    
    //inventory

    public void SetWeapon(PlayerWeaponData data)
    {
        CurrentWeaponData = data;
    }

    public void AddWeaponData(PlayerWeaponData data)
    {
        WeaponDataList.Add(data);
        weaponInventoryCount = WeaponDataList.Count;
        WeaponDataList.Sort((a, b)
            => string.Compare(a.WeaponName, b.WeaponName, StringComparison.Ordinal));
    }

    public void AddEquipmentData(PlayerEquipmentData data)
    {
        EquipmentDataList.Add(data);
        equipmentInventoryCount = EquipmentDataList.Count;
        EquipmentDataList.Sort( (a,b) 
            => string.Compare(a.EquipmentName, b.EquipmentName, StringComparison.Ordinal));
    }

    public void AddConsumableData(ConsumableItemData data)
    {
        var existingItem = ConsumableDataList.FirstOrDefault(x => x.ItemData == data);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            ConsumableDataList.Add(new ConsumableItem { ItemData = data, Quantity = 1 });
            consumableInventoryCount = ConsumableDataList.Count;
            ConsumableDataList.Sort( (a,b)
                => string.Compare(a.ItemData.ItemName, b.ItemData.ItemName, StringComparison.Ordinal));
        }
    }
    
    public void AddMoney(int amount)
    {
        _moneyAmount += amount;
        UpdateMoneyAmount?.Invoke(_moneyAmount);
    }
    
    private void Awake()
    {
        weaponInventoryCount = 0;
        weaponInventoryMaxCount = 20;
        
        equipmentInventoryCount = 0;
        equipmentInventoryMaxCount = 20;
        
        consumableInventoryCount = 0;
        consumableInventoryMaxCount = 20;

        consumableItemMaxQuantity = 10;//현재-모든 소비템 최대 보유 개수 통일

        _moneyAmount = 0;
    }
}
