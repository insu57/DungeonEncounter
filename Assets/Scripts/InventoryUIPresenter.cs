using System.Collections;
using System.Collections.Generic;
using Player;
using Scriptable_Objects;
using Sirenix.Reflection.Editor;
using UI;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
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
        PlayerWeaponData data = _inventoryManager.CurrentWeaponData.Weapon.ItemData;
        if (data != null)
        {
            _inventoryUIView.SelectedWeapon(data);
            _inventoryUIView.SetQuickSlotBtnActive(false);
            _inventoryUIView.ToggleItemEquipBtn(false); //무기는 무조건 장착해야함.(장착해제라고 표시x)
            _inventoryUIView.SetEquipBtnActive(false);
            _inventoryUIView.SetDropBtnActive(false);
        
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
            _inventoryManager.SelectedItem.Weapon = _inventoryManager.CurrentWeaponData.Weapon;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
    }
    
    private void HandleShowCurrentEquipment()
    {
        PlayerEquipmentData data = _inventoryManager.CurrentEquipmentData.Equipment.ItemData;
        if (data != null)
        { 
            _inventoryUIView.SelectedEquipment(data);
            _inventoryUIView.SetQuickSlotBtnActive(false);
            _inventoryUIView.ToggleItemEquipBtn(true);
            _inventoryUIView.SetEquipBtnActive(true);
            _inventoryUIView.SetDropBtnActive(false);
            
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
            _inventoryManager.SelectedItem.Equipment = _inventoryManager.CurrentEquipmentData.Equipment;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }
    private void HandleShowCurrentQuick1()
    {
        InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ItemQuickSlot1.Consumable;
        if (item.ItemData != null)
        {
            _inventoryUIView.SelectedConsumable(item);
            _inventoryUIView.SetQuickSlotBtnActive(true);
            _inventoryUIView.ToggleQuick1Btn(true);
            _inventoryUIView.ToggleQuick2Btn(item.ItemData == _inventoryManager.ItemQuickSlot2.Consumable.ItemData);
            _inventoryUIView.SetDropBtnActive(false);
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot1.Consumable;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }

    private void HandleShowCurrentQuick2()
    {
        InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ItemQuickSlot2.Consumable;
        if (item.ItemData != null)
        {
            _inventoryUIView.SelectedConsumable(item);
            _inventoryUIView.SetQuickSlotBtnActive(true);
            _inventoryUIView.ToggleQuick1Btn(item.ItemData == _inventoryManager.ItemQuickSlot1.Consumable.ItemData);
            _inventoryUIView.ToggleQuick2Btn(true);
            _inventoryUIView.SetDropBtnActive(false);    
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot2.Consumable;
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
                    bool isEquipped = item.Index == _inventoryManager.CurrentWeaponData.Weapon.Index 
                                      && _inventoryManager.CurrentWeaponData.IsEquipped;
                    _inventoryUIView.SelectedWeapon(item.ItemData);//아이템 인덱스로 구분
                    if (item.ItemData.IsDefaultWeapon || isEquipped) //장착한 무기나 기본 무기는 드랍불가
                    {
                        _inventoryUIView.SetDropBtnActive(false);
                    }
                    else
                    {
                        _inventoryUIView.SetDropBtnActive(true);
                    }
                    _inventoryUIView.SetQuickSlotBtnActive(false);
                    _inventoryUIView.SetEquipBtnActive(!isEquipped);
                    _inventoryUIView.ToggleItemEquipBtn(false);
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
                    _inventoryManager.SelectedItem.Weapon = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Equipment:
                if (index < _inventoryManager.equipmentInventoryCount)
                {
                    InventoryManager.EquipmentDataWithIndex item = _inventoryManager.EquipmentDataList[index];
                    bool isEquipped = item.Index == _inventoryManager.CurrentEquipmentData.Equipment.Index 
                                      && _inventoryManager.CurrentEquipmentData.IsEquipped;
                    
                    _inventoryUIView.SelectedEquipment(item.ItemData);
                    _inventoryUIView.SetQuickSlotBtnActive(false);
                    _inventoryUIView.SetEquipBtnActive(true);
                    
                    _inventoryUIView.ToggleItemEquipBtn(isEquipped);
                    _inventoryUIView.SetDropBtnActive(!isEquipped);
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
                    _inventoryManager.SelectedItem.Equipment = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Consumable:
                if (index < _inventoryManager.consumableInventoryCount)//소모템은 인덱스없음(중복가능해서 현재는 없음)
                {
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ConsumableDataList[index];
                    _inventoryUIView.SelectedConsumable(item);
                    //퀵슬롯 설정
                    bool isQuick1 = item.ItemData == _inventoryManager.ItemQuickSlot1.Consumable.ItemData 
                                    && _inventoryManager.ItemQuickSlot1.IsEquipped;
                    bool isQuick2 = item.ItemData == _inventoryManager.ItemQuickSlot2.Consumable.ItemData 
                                    && _inventoryManager.ItemQuickSlot2.IsEquipped;
                    _inventoryUIView.SetQuickSlotBtnActive(true);
                    
                    _inventoryUIView.ToggleQuick1Btn(isQuick1);
                    _inventoryUIView.ToggleQuick2Btn(isQuick2);
                    _inventoryUIView.SetDropBtnActive(!(isQuick1||isQuick2)); //수량따라 작동하게 수정필요
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
                    _inventoryManager.SelectedItem.Consumable = item;
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
            case ItemTypes.Weapon: //무기...항상 하나는 장착
            {
                InventoryManager.WeaponDataWithIndex dataWithIndex = _inventoryManager.SelectedItem.Weapon;
                _inventoryManager.SetWeapon(dataWithIndex.ItemData,dataWithIndex.Index);
                _inventoryUIView.UpdateCurrentWeapon(dataWithIndex.ItemData.Icon);
                _inventoryUIView.SetEquipBtnActive(false);
                _inventoryUIView.SetDropBtnActive(false);
                break;
            }
            case ItemTypes.Equipment:
            {
                if (_inventoryManager.SelectedItem.IsEquipped)
                {
                    _inventoryManager.SelectedItem.IsEquipped = false;  //장착 해제
                    _inventoryManager.CurrentEquipmentData.IsEquipped = false; 
                    //현재 장착장비 해제 bool(데이터는 남음) or null값? 새로운 클래스?
                    _inventoryUIView.ToggleItemEquipBtn(false); //장착버튼 토글(장착/해제 텍스트)
                    _inventoryUIView.SetDropBtnActive(true); //드랍 가능
                    _inventoryUIView.InactiveCurrentEquipment(); //PlayerStatus에서 비활성
                }
                else
                {
                    InventoryManager.EquipmentDataWithIndex dataWithIndex = _inventoryManager.SelectedItem.Equipment;
                    _inventoryManager.SetEquipment(dataWithIndex.ItemData,dataWithIndex.Index);
                    _inventoryManager.SelectedItem.IsEquipped = true;
                    
                    _inventoryUIView.UpdateCurrentEquipment(dataWithIndex.ItemData.Icon);
                    _inventoryUIView.ToggleItemEquipBtn(true);
                    _inventoryUIView.SetDropBtnActive(false);
                }
                
                break;
            }
            default:
                break;
        }
    }

    private void HandleOnSetQuickSlot(int quickSlot)
    {
        //퀵슬롯1,2 확인...
        bool isQuick1 = _inventoryManager.SelectedItem.Consumable.ItemData == 
                        _inventoryManager.ItemQuickSlot1.Consumable.ItemData 
                        && _inventoryManager.ItemQuickSlot1.IsEquipped;
        bool isQuick2 = _inventoryManager.SelectedItem.Consumable.ItemData ==
                        _inventoryManager.ItemQuickSlot2.Consumable.ItemData
                        && _inventoryManager.ItemQuickSlot2.IsEquipped;
        switch (quickSlot)
        {
            case 1:
                if (isQuick1)
                {
                    _inventoryManager.SelectedItem.IsEquipped = isQuick2;
                    _inventoryManager.ItemQuickSlot1.IsEquipped = false;
                    _inventoryUIView.SetDropBtnActive(!isQuick2);
                    _inventoryUIView.ToggleQuick1Btn(false);
                    _inventoryUIView.InactiveCurrentQuick1();
                }
                else
                {
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.SelectedItem.Consumable;
                    _inventoryManager.SelectedItem.IsEquipped = true;
                    _inventoryManager.SetQuickSlot1(item);
                    
                    _inventoryUIView.UpdateCurrentQuick1(item.ItemData.Icon);
                    _inventoryUIView.SetDropBtnActive(false);
                    _inventoryUIView.ToggleQuick1Btn(true);
                }
                break;
            case 2:
                if (isQuick2)
                {
                    _inventoryManager.SelectedItem.IsEquipped = isQuick1;
                    _inventoryManager.ItemQuickSlot2.IsEquipped = false;
                    _inventoryUIView.SetDropBtnActive(!isQuick1);
                    _inventoryUIView.ToggleQuick2Btn(false);
                    _inventoryUIView.InactiveCurrentQuick2();
                }
                else
                {
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.SelectedItem.Consumable;
                    _inventoryManager.SelectedItem.IsEquipped = true;
                    _inventoryManager.SetQuickSlot2(item);
                    
                    _inventoryUIView.UpdateCurrentQuick2(item.ItemData.Icon);
                    _inventoryUIView.SetDropBtnActive(false);
                    _inventoryUIView.ToggleQuick2Btn(true);
                }
                break;
            default:
                break;
        }
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
