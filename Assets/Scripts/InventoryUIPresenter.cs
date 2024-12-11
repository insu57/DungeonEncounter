using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUIPresenter
{
    private readonly PlayerManager _playerManager;
    private readonly InventoryManager _inventoryManager;
    private readonly InventoryUIView _inventoryUIView;

    public InventoryUIPresenter(PlayerManager playerManager, InventoryManager inventoryManager,
        InventoryUIView inventoryUIView)
    {
        _playerManager = playerManager;
        _inventoryManager = inventoryManager;
        _inventoryUIView = inventoryUIView;

        _inventoryUIView.OnInventoryOpen += HandleOnInventoryOpen;
        _inventoryUIView.OnCurrentWeapon += HandleShowCurrentWeapon;
        _inventoryUIView.OnCurrentEquipment += HandleShowCurrentEquipment;
        _inventoryUIView.OnQuickSlot1 += HandleShowCurrentQuick1;
        _inventoryUIView.OnQuickSlot2 += HandleShowCurrentQuick2;
        _inventoryUIView.OnSelectInventorySlot += HandleSelectInventorySlot;

        _playerManager.OnGetMoney += HandleAddMoney;
        _playerManager.OnGetItem += HandleAddItem;
        _inventoryManager.UpdateMoneyAmount += HandleUpdateTotalMoney;
        
        //init
      
        _inventoryManager.AddWeaponData(_playerManager.WeaponData);
        _inventoryManager.SetWeapon(_playerManager.WeaponData);
        _inventoryUIView.UpdateCurrentWeapon(_playerManager.WeaponData.Icon);
        int maxCount = _inventoryManager.weaponInventoryMaxCount;  //생성은 초기화, 최대칸 증가 시에만
        for (int i = 0; i < maxCount; i++)
        {
            _inventoryUIView.InitInventory();
        }
        //_playerUIView.UpdateInventoryIcon(0, _playerManager.WeaponData.Icon);

    }
    
    private void HandleShowCurrentWeapon()
    {
        PlayerWeaponData data = _inventoryManager.CurrentWeaponData;
        _inventoryUIView.SelectedWeapon(data);
        _inventoryUIView.ItemButtonActive(true);
    }
    
    private void HandleShowCurrentEquipment()
    {
        PlayerEquipmentData data = _inventoryManager.CurrentEquipmentData;
        if (data != null)
        {
            _inventoryUIView.SelectedEquipment(data);
            _inventoryUIView.ItemButtonActive(true);
        }
        
    }
    private void HandleShowCurrentQuick1()
    {
        ConsumableItemData data = _inventoryManager.ItemQuickSlot1;
        if (data != null)
        {
            _inventoryUIView.SelectedConsumable(data);
            _inventoryUIView.ItemButtonActive(true);
        }
        
    }

    private void HandleShowCurrentQuick2()
    {
        ConsumableItemData data = _inventoryManager.ItemQuickSlot2;
        if (data != null)
        {
            _inventoryUIView.SelectedConsumable(data);
            _inventoryUIView.ItemButtonActive(true);
        }
    }

    private void HandleSelectInventorySlot(ItemTypes type, int index)
    {
        switch (type)
        {
            case ItemTypes.Weapon:
                if (index < _inventoryManager.weaponInventoryCount)
                {
                    PlayerWeaponData data = _inventoryManager.WeaponDataList[index];
                    _inventoryUIView.SelectedWeapon(data);
                    _inventoryUIView.ItemButtonActive(true);
                }
                break;
            case ItemTypes.Equipment:
                if (index < _inventoryManager.equipmentInventoryCount)
                {
                    PlayerEquipmentData data = _inventoryManager.EquipmentDataList[index];
                    _inventoryUIView.SelectedEquipment(data);
                    _inventoryUIView.ItemButtonActive(true);
                }
                break;
            case ItemTypes.Consumable:
                if (index < _inventoryManager.consumableInventoryCount)
                {
                    ConsumableItemData data = _inventoryManager.ConsumableDataList[index].ItemData;
                    //int quantity = _inventoryManager.ConsumableDataList[index].Quantity;
                    _inventoryUIView.SelectedConsumable(data);
                    _inventoryUIView.ItemButtonActive(true);
                }
                break;
            default:
                break;
        }
    }
    
    private void HandleOnInventoryOpen(ItemTypes type)
    {
        int count, maxCount;
        switch (type)
        {
            case ItemTypes.Weapon:
                count = _inventoryManager.weaponInventoryCount;
                maxCount = _inventoryManager.weaponInventoryMaxCount;
                _inventoryUIView.UpdateInventoryCount(count, maxCount);
                _inventoryUIView.ClearInventoryIcon(maxCount);
                int index = 0;
                foreach (var weaponData in _inventoryManager.WeaponDataList)
                {
                    _inventoryUIView.UpdateInventoryIcon(index, weaponData.Icon);
                    index++;
                }
                
                break;
            case ItemTypes.Equipment:
                count = _inventoryManager.equipmentInventoryCount;
                maxCount = _inventoryManager.equipmentInventoryMaxCount;
                _inventoryUIView.UpdateInventoryCount(count, maxCount);
                _inventoryUIView.ClearInventoryIcon(maxCount);
                index = 0;
                foreach (var equipmentData in _inventoryManager.EquipmentDataList)
                {
                    _inventoryUIView.UpdateInventoryIcon(index, equipmentData.Icon);
                    index++;
                }
                break;
            case ItemTypes.Consumable:
                count = _inventoryManager.consumableInventoryCount;
                maxCount = _inventoryManager.consumableInventoryMaxCount;
                _inventoryUIView.UpdateInventoryCount(count, maxCount);
                _inventoryUIView.ClearInventoryIcon(maxCount);
                index = 0;
                foreach (var consumableItem in _inventoryManager.ConsumableDataList)
                {
                    int quantity = consumableItem.Quantity;
                    _inventoryUIView.UpdateInventoryIconWithQuantity(index, consumableItem.ItemData.Icon, quantity);
                }
                break;
        }
        
        
    }

    private void HandleAddMoney(int money)
    {
        _inventoryManager.AddMoney(money);
    }

    private void HandleAddItem(GameObject item)
    {
        if (item.layer == (int)ItemLayers.Weapon)
        {
            PlayerWeaponData weaponData = item.GetComponent<PlayerWeapon>().WeaponData;
            _inventoryManager.AddWeaponData(weaponData);
        }
        else if (item.layer == (int)ItemLayers.Equipment)
        {
            PlayerEquipmentData equipmentData = item.GetComponent<PlayerEquipment>().Data;
            _inventoryManager.AddEquipmentData(equipmentData);
        }
        else if (item.layer == (int)ItemLayers.Consumable)
        {
            ConsumableItemData consumableData = item.GetComponent<ConsumableItem>().Data;
            _inventoryManager.AddConsumableData(consumableData);
        }
    }

    private void HandleUpdateTotalMoney(int moneyAmount)
    {
        _inventoryUIView.UpdateMoney(moneyAmount);
        
    }
    
    public void Dispose()
    {
        _inventoryUIView.OnInventoryOpen -= HandleOnInventoryOpen;
        //
        _inventoryUIView.OnSelectInventorySlot -= HandleSelectInventorySlot;
    }
}
