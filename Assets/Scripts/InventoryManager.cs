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
    public class WeaponDataWithIndex
    {
        public PlayerWeaponData ItemData;
        public int Index;
    }

    public class EquipmentDataWithIndex
    {
        public PlayerEquipmentData ItemData;
        public int Index;
    }
    public class ConsumableDataWithQuantity
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
    public WeaponDataWithIndex CurrentWeaponData { private set; get; } = new WeaponDataWithIndex();
    public EquipmentDataWithIndex CurrentEquipmentData { private set; get; } = new EquipmentDataWithIndex();
    public ConsumableDataWithQuantity ItemQuickSlot1 { private set; get; } = new ConsumableDataWithQuantity();
    public ConsumableDataWithQuantity ItemQuickSlot2 { private set; get; } = new ConsumableDataWithQuantity();

    private GameObject _selectedItem; //GameObject?Data?...
    public List<WeaponDataWithIndex> WeaponDataList { get; } = new List<WeaponDataWithIndex>();
    public List<EquipmentDataWithIndex> EquipmentDataList { get; } = new List<EquipmentDataWithIndex>();
    public List<ConsumableDataWithQuantity> ConsumableDataList { get; } = new List<ConsumableDataWithQuantity>();

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

    public void SetWeapon(PlayerWeaponData data, int index)
    {
        CurrentWeaponData.ItemData = data;
        CurrentWeaponData.Index = index;
    }

    public void SetEquipment(PlayerEquipmentData data, int index)
    {
        CurrentEquipmentData.ItemData = data;
        CurrentEquipmentData.Index = index;
    }

    public void SetQuickSlot1(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot1 = data;
    }

    public void SetQuickSlot2(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot2 = data;
    }

    public void AddWeaponData(PlayerWeaponData data)
    {
        WeaponDataList.Add(new WeaponDataWithIndex { ItemData = data, Index = weaponInventoryCount });
        weaponInventoryCount = WeaponDataList.Count;
        WeaponDataList.Sort((a, b)
            => string.Compare(a.ItemData.WeaponName, b.ItemData.WeaponName, StringComparison.Ordinal));
    }

    public void AddEquipmentData(PlayerEquipmentData data)
    {
        EquipmentDataList.Add(new EquipmentDataWithIndex { ItemData = data, Index = equipmentInventoryCount });
        equipmentInventoryCount = EquipmentDataList.Count;
        EquipmentDataList.Sort( (a,b) 
            => string.Compare(a.ItemData.EquipmentName, b.ItemData.EquipmentName, StringComparison.Ordinal));
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
            ConsumableDataList.Add(new ConsumableDataWithQuantity { ItemData = data, Quantity = 1 });
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
