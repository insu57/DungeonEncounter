using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
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
        
        _playerUIView.OnWeaponChanged += HandleWeaponChanged;
        _playerUIView.OnEquipmentChanged += HandleEquipmentChanged;
        _playerUIView.OnItemQuickChanged += HandleItemQuickChanged;
        _playerUIView.OnShowIcon += HandleShowIcon;
        
        //init
      
        _inventoryManager.SetWeapon(_playerManager.WeaponData);
        _playerUIView.UpdateCurrentWeapon(_playerManager.WeaponData.Icon);
    }
    
    private void HandleShowIcon()
    {
        
        //_playerManager.
    }
            
    private void HandleWeaponChanged()
    {
        //_inventoryManager.SetWeapon();

    }
    private void HandleEquipmentChanged()
    {


    }
    private void HandleItemQuickChanged(int index)
    {
        switch (index)
        {
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }
}
