using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class TraderNpcUIPresenter
{
    private readonly TraderNpc _traderNpc;
    private readonly TraderNpcUI _traderNpcUI;
    private readonly InventoryManager _inventoryManager;
    
    public TraderNpcUIPresenter(TraderNpc traderNpc, TraderNpcUI traderNpcUI, InventoryManager inventoryManager)
    {
        _traderNpc = traderNpc;
        _traderNpcUI = traderNpcUI;
        _inventoryManager = inventoryManager;

        _inventoryManager.OnUpdateMoneyAmount += HandleOnUpdateMoneyAmount;
        _traderNpcUI.OnOnRerollBtnClicked += HandleOnRerollBtnClicked;
        _traderNpcUI.OnItemPurchasedBtnClicked += HandleOnItemPurchased;
        
        //Initialize
        RerollItemData();
        var rerollCost = _traderNpc.GetRerollCost();
        _traderNpcUI.UpdateRerollCostTxt(rerollCost, _inventoryManager.UseMoney(rerollCost, true)
            ? Color.white : Color.red);
    }

    private void RerollItemData()
    {
        _traderNpc.SetItemData(1);
        _traderNpc.SetItemData(2);
        _traderNpc.SetItemData(3); //Item1~3 Random Item
        if (_traderNpc.GetItemData(1) is ConsumableItemData consumableItemData)
        {
            _traderNpcUI.RerollItem1(consumableItemData, _traderNpc.GetItemPrice(1));
        }
        if (_traderNpc.GetItemData(2) is PlayerWeaponData playerWeaponData)
        {
            _traderNpcUI.RerollItem2(playerWeaponData, _traderNpc.GetItemPrice(2));
        }
        if (_traderNpc.GetItemData(3) is PlayerEquipmentData playerEquipmentData)
        {
            _traderNpcUI.RerollItem3(playerEquipmentData, _traderNpc.GetItemPrice(3));
        }
        //
        _traderNpcUI.UpdateItemPriceTxtColor(1, _inventoryManager.UseMoney(_traderNpc.GetItemPrice(1), true) 
            ? Color.white : Color.red);
        _traderNpcUI.UpdateItemPriceTxtColor(2, _inventoryManager.UseMoney(_traderNpc.GetItemPrice(2), true)
            ? Color.white : Color.red);
        _traderNpcUI.UpdateItemPriceTxtColor(3, _inventoryManager.UseMoney(_traderNpc.GetItemPrice(3),true)
            ? Color.white : Color.red);
    }

    private void HandleOnRerollBtnClicked()
    {
        var rerollCost = _traderNpc.GetRerollCost();
        _inventoryManager.UseMoney(rerollCost, false);
        
        RerollItemData();
        _traderNpc.IncreaseRerollCost();
        rerollCost = _traderNpc.GetRerollCost();
        _traderNpcUI.UpdateRerollCostTxt(rerollCost, _inventoryManager.UseMoney(rerollCost,true) 
            ? Color.white : Color.red);
    }

    private void HandleOnUpdateMoneyAmount(int moneyAmount)
    {
        _traderNpcUI.UpdateCurrentMoney(moneyAmount);
        var rerollCost = _traderNpc.GetRerollCost();
        _traderNpcUI.UpdateRerollCostTxt(rerollCost, rerollCost <= moneyAmount ? Color.white : Color.red);
        _traderNpcUI.UpdateItemPriceTxtColor(1, _traderNpc.GetItemPrice(1) <= moneyAmount ? Color.white : Color.red);
        _traderNpcUI.UpdateItemPriceTxtColor(2, _traderNpc.GetItemPrice(2) <= moneyAmount ? Color.white : Color.red);
        _traderNpcUI.UpdateItemPriceTxtColor(3, _traderNpc.GetItemPrice(3) <= moneyAmount ? Color.white : Color.red);
    }

    private void HandleOnItemPurchased(int itemIdx)
    {
        if (_inventoryManager.UseMoney(_traderNpc.GetItemPrice(itemIdx), false))
        {
            _traderNpcUI.ItemSoldOut(itemIdx);
            _traderNpc.SpawnPurchasedItem(itemIdx);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.MoneyPickupSfx);
        }
    }
    
    public void Dispose()
    {
        _inventoryManager.OnUpdateMoneyAmount -= HandleOnUpdateMoneyAmount;
        _traderNpcUI.OnOnRerollBtnClicked -= HandleOnRerollBtnClicked;
        _traderNpcUI.OnItemPurchasedBtnClicked -= HandleOnItemPurchased;
    }
}
