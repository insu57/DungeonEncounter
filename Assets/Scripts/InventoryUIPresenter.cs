using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using Sirenix.Reflection.Editor;
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
        _inventoryManager.OnUpdateMoneyAmount += HandleUpdateTotalMoney;
        //InventoryManager StatChange -> PlayerManager
        _inventoryUIView.OnEquipButton += HandleOnEquipButton;
        _inventoryUIView.OnDropButton += HandleOnDropButton;
        _inventoryUIView.OnSetQuickSlot += HandleOnSetQuickSlot;
        
        
        //init
        _inventoryManager.AddWeaponData(_playerManager.WeaponData);
        _inventoryManager.SetWeapon(_playerManager.WeaponData, 0);
        _inventoryUIView.UpdateCurrentWeapon(_playerManager.WeaponData.Icon);
        int maxCount = _inventoryManager.weaponInventoryMaxCount;  //생성은 초기화, 최대칸 증가 시에만
        for (int i = 0; i < maxCount; i++)
        {
            _inventoryUIView.InitInventory();
        }
        //_playerUIView.UpdateInventoryIcon(0, _playerManager.WeaponData.Icon);

    }
    
    private void HandleShowCurrentWeapon() //현재 무기 데이터 표시
    {
        PlayerWeaponData data = _inventoryManager.CurrentWeaponData.ItemData;
        _inventoryUIView.SelectedWeapon(data);
        _inventoryUIView.SetQuickSlotActive(false);
        _inventoryUIView.SetItemEquipButton(true);
        _inventoryUIView.ItemEquipButtonActive(true);
        _inventoryUIView.ItemDropButtonActive(false);
        
        _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
        _inventoryManager.SelectedItem.Weapon = _inventoryManager.CurrentWeaponData;
        _inventoryManager.SelectedItem.IsEquipped = true;
    }
    
    private void HandleShowCurrentEquipment()
    {
        PlayerEquipmentData data = _inventoryManager.CurrentEquipmentData.ItemData;
        if (data != null)
        { 
            _inventoryUIView.SelectedEquipment(data);
            _inventoryUIView.SetQuickSlotActive(false);
            _inventoryUIView.SetItemEquipButton(true);
            _inventoryUIView.ItemEquipButtonActive(true);
            _inventoryUIView.ItemDropButtonActive(false);
            
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
            _inventoryManager.SelectedItem.Equipment = _inventoryManager.CurrentEquipmentData;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }
    private void HandleShowCurrentQuick1()
    {
        InventoryManager.ConsumableDataWithQuantity data = _inventoryManager.ItemQuickSlot1;
        if (data != null)
        {
            _inventoryUIView.SelectedConsumable(data);
            _inventoryUIView.SetQuickSlotActive(true);
            _inventoryUIView.SetQuick1Button(true);
            _inventoryUIView.SetQuick2Button(false);
            _inventoryUIView.ItemDropButtonActive(false);
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot1;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }

    private void HandleShowCurrentQuick2()
    {
        InventoryManager.ConsumableDataWithQuantity data = _inventoryManager.ItemQuickSlot2;
        if (data != null)
        {
            _inventoryUIView.SelectedConsumable(data);
            _inventoryUIView.SetQuickSlotActive(true);
            _inventoryUIView.SetQuick1Button(false);
            _inventoryUIView.SetQuick2Button(true);
            _inventoryUIView.ItemDropButtonActive(false);    
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot2;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
    }

    private void HandleSelectInventorySlot(ItemTypes type, int index) //선택한 인벤토리 슬롯 아이템 데이터 표시
    {
        switch (type)
        {
            case ItemTypes.Weapon:
                if (index < _inventoryManager.weaponInventoryCount)
                {
                    InventoryManager.WeaponDataWithIndex item = _inventoryManager.WeaponDataList[index];
                    bool isEquipped = item.Index == _inventoryManager.CurrentWeaponData.Index;
                    _inventoryUIView.SelectedWeapon(item.ItemData);//아이템 인덱스로 구분
                    if (item.ItemData.IsDefaultWeapon || isEquipped)
                    {
                        _inventoryUIView.ItemDropButtonActive(false);
                    }
                    else
                    {
                        _inventoryUIView.ItemDropButtonActive(true);
                    }
                    _inventoryUIView.SetQuickSlotActive(false);
                    _inventoryUIView.ItemEquipButtonActive(true);
                    
                    _inventoryUIView.SetItemEquipButton(isEquipped);
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
                    _inventoryManager.SelectedItem.Weapon = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Equipment:
                if (index < _inventoryManager.equipmentInventoryCount)
                {
                    InventoryManager.EquipmentDataWithIndex item = _inventoryManager.EquipmentDataList[index];
                    bool isEquipped = _inventoryManager.CurrentEquipmentData.ItemData && item.Index == _inventoryManager.CurrentEquipmentData.Index;
                    
                    _inventoryUIView.SelectedEquipment(item.ItemData);
                    _inventoryUIView.SetQuickSlotActive(false);
                    _inventoryUIView.ItemEquipButtonActive(true);
                    
                    _inventoryUIView.SetItemEquipButton(isEquipped);
                    _inventoryUIView.ItemDropButtonActive(!isEquipped);
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
                    _inventoryManager.SelectedItem.Equipment = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Consumable:
                if (index < _inventoryManager.consumableInventoryCount)//소모템은 인덱스없음(중복가능해서 현재는 없음)
                {
                    InventoryManager.ConsumableDataWithQuantity data = _inventoryManager.ConsumableDataList[index];
                    _inventoryUIView.SelectedConsumable(data);
                    //퀵슬롯 설정
                    bool isQuick1 = _inventoryManager.ItemQuickSlot1.ItemData && data == _inventoryManager.ItemQuickSlot1;
                    bool isQuick2 = _inventoryManager.ItemQuickSlot1.ItemData && data == _inventoryManager.ItemQuickSlot2;
                    _inventoryUIView.SetQuickSlotActive(true);
                    
                    _inventoryUIView.SetQuick1Button(isQuick1);
                    _inventoryUIView.SetQuick2Button(isQuick2);
                    _inventoryUIView.ItemDropButtonActive(!(isQuick1||isQuick2)); //수량따라 작동하게 수정필요
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
                    _inventoryManager.SelectedItem.Consumable = data;
                    _inventoryManager.SelectedItem.IsEquipped = isQuick1 || isQuick2;
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
                    _inventoryUIView.UpdateInventoryIcon(index, weaponData.ItemData.Icon);
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
                    _inventoryUIView.UpdateInventoryIcon(index, equipmentData.ItemData.Icon);
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
                    index++;
                    Debug.Log(consumableItem.ItemData.ItemName + ' ' + index);
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

    private void HandleOnEquipButton()
    {
        ItemTypes itemType = _inventoryManager.SelectedItem.ItemType;
        switch (itemType)
        {
            case ItemTypes.Weapon:
            {
                InventoryManager.WeaponDataWithIndex dataWithIndex = _inventoryManager.SelectedItem.Weapon;
                _inventoryManager.SetWeapon(dataWithIndex.ItemData,dataWithIndex.Index);
                _inventoryUIView.UpdateCurrentWeapon(dataWithIndex.ItemData.Icon);
                break;
            }
            case ItemTypes.Equipment:
            {
                PlayerEquipmentData data = _inventoryManager.SelectedItem.Equipment.ItemData;
                Debug.Log(data.EquipmentName);
                break;
            }
            case ItemTypes.Consumable:
            {
                ConsumableItemData data = _inventoryManager.SelectedItem.Consumable.ItemData;
                Debug.Log(data.ItemName);
                break;
            }
            default:
                break;
        }
    }

    private void HandleOnSetQuickSlot(int quickSlot)
    {
        
    }
    
    private void HandleOnDropButton()
    {
        
    }
    public void Dispose()
    {
        _inventoryUIView.OnInventoryOpen -= HandleOnInventoryOpen;
        //
        _inventoryUIView.OnSelectInventorySlot -= HandleSelectInventorySlot;
    }
}
