using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUIPresenter
{
    private readonly PlayerManager _playerManager;
    private readonly InventoryManager _inventoryManager;
    private readonly PlayerUIView _playerUIView;

    public InventoryUIPresenter(PlayerManager playerManager, InventoryManager inventoryManager, PlayerUIView playerUIView)
    {
        _playerManager = playerManager;
        _inventoryManager = inventoryManager;
        _playerUIView = playerUIView;

        _playerUIView.OnInventoryOpen += HandleOnInventoryOpen;
        _playerUIView.OnWeaponInventory += HandleOnWeaponInventory;
        _playerUIView.OnEquipmentInventory += HandleOpenEquipmentInventory;
        _playerUIView.OnConsumableInventory += HandleOpenConsumableInventory;
        _playerUIView.OnShowIcon += HandleShowIcon;
        
        //init
      
        _inventoryManager.AddWeaponData(_playerManager.WeaponData);
        _inventoryManager.SetWeapon(_playerManager.WeaponData);
        _playerUIView.UpdateCurrentWeapon(_playerManager.WeaponData.Icon);
        int maxCount = inventoryManager.weaponInventoryMaxCount;  //생성은 초기화, 최대칸 증가 시에만
        for (int i = 0; i < maxCount; i++)
        {
            _playerUIView.InitInventory();
        }
        _playerUIView.UpdateInventoryIcon(0, _playerManager.WeaponData.Icon);

    }
    
    private void HandleShowIcon()
    {
        
        //_playerManager.
    }

    private void HandleOnInventoryOpen(ItemTypes type)
    {
        int count, maxCount;
        switch (type)
        {
            case ItemTypes.Weapon:
                count = _inventoryManager.weaponInventoryCount;
                maxCount = _inventoryManager.weaponInventoryMaxCount;
                _playerUIView.UpdateInventoryCount(count, maxCount);
                
                break;
            case ItemTypes.Equipment:
                count = _inventoryManager.equipmentInventoryCount;
                maxCount = _inventoryManager.equipmentInventoryMaxCount;
                _playerUIView.UpdateInventoryCount(count, maxCount);
                break;
            case ItemTypes.Consumable:
                count = _inventoryManager.consumableInventoryCount;
                maxCount = _inventoryManager.consumableInventoryMaxCount;
                _playerUIView.UpdateInventoryCount(count, maxCount);
                break;
        }
        
        
    }
    
    private void HandleOnWeaponInventory()
    {
        
    }
    private void HandleOpenEquipmentInventory()
    {


    }
    private void HandleOpenConsumableInventory()
    {
    }

    public void Dispose()
    {
        _playerUIView.OnInventoryOpen -= HandleOnInventoryOpen;
        //
        _playerUIView.OnShowIcon -= HandleShowIcon;
    }
}
