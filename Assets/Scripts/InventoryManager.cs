using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    private GameObject _currentEquipment;
    private GameObject _itemQuickSlot1;
    private GameObject _itemQuickSlot2;
    
    public PlayerWeaponData CurrentWeaponData { private set; get; }
    public PlayerEquipmentData CurrentEquipmentData { private set; get; }
    public ConsumableItemData ItemQuickSlot1 { private set; get; }
    public ConsumableItemData ItemQuickSlot2 { private set; get; }

    private GameObject _selectedItem; //GameObject?Data?...
    public List<PlayerWeaponData> WeaponDataList { private set; get; }
    public List<PlayerEquipmentData> EquipmentDataList { private set; get;}
    public List<ConsumableItemData> ConsumableDataList { private set; get; } //수량도 필요함..Dictionary?
    
    //public Action 
    //public Event changeWeapon;
    //public Event changeEquipment;
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
    
    
    private void Awake()
    {
        weaponInventoryCount = 0;
        weaponInventoryMaxCount = 32;
        
        equipmentInventoryCount = 0;
        equipmentInventoryMaxCount = 32;
        
        consumableInventoryCount = 0;
        consumableInventoryMaxCount = 32;

        consumableItemMaxQuantity = 10;//현재-모든 소비템 최대 보유 개수 통일
        
    }
    
    private void Update()
    {
        
    }
}
