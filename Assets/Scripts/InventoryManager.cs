using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    private GameObject _currentEquipment;
    private GameObject _itemQuickSlot1;
    private GameObject _itemQuickSlot2;
    
    public ItemData CurrentWeaponData { private set; get; }
    public PlayerEquipmentData CurrentEquipmentData { private set; get; }
    public ConsumableItemData ItemQuickSlot1 { private set; get; }
    public ConsumableItemData ItemQuickSlot2 { private set; get; }

    private GameObject _selectedItem; //GameObject?Data?...
    private List<ItemData> _weaponDataList = new List<ItemData>();
    private List<PlayerEquipmentData> _equipmentDataList = new List<PlayerEquipmentData>();
    private List<ConsumableItemData> _itemDataList = new List<ConsumableItemData>(); //수량도 필요함..Dictionary?
    
    //public Action 
    //public Event changeWeapon;
    //public Event changeEquipment;
    public Event UpdateInventory;
    
    private int _weaponInventoryCount;
    private int _weaponInventoryMaxCount;
    
    private int _equipmentInventoryCount;
    private int _equipmentInventoryMaxCount;
    
    private int _consumableInventoryCount;
    private int _consumableInventoryMaxCount;
    //아이템별 현재 수량...
    private int _consumableItemMaxQuantity;
    
    
    //inventory

    public void SetWeapon(ItemData data)
    {
        if(data.ItemType != ItemTypes.Weapon) return;
        CurrentWeaponData = data;
        _weaponDataList.Add(data);
    }
    
    private void Awake()
    {
        _weaponInventoryCount = 0;
        _weaponInventoryMaxCount = 32;
        
        _equipmentInventoryCount = 0;
        _equipmentInventoryMaxCount = 32;
        
        _consumableInventoryCount = 0;
        _consumableInventoryMaxCount = 32;

        _consumableItemMaxQuantity = 10;//현재-모든 소비템 최대 보유 개수 통일
        
    }
    
    private void Update()
    {
        
    }
}
