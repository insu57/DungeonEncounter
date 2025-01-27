using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderNpcUI : MonoBehaviour
{
    private InventoryManager _inventoryManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private TraderNpc traderNpc;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject traderUI;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    
    [Header("Item 1")]
    [SerializeField] private Image itemImage1;
    [SerializeField] private TextMeshProUGUI itemName1Txt;
    [SerializeField] private TextMeshProUGUI itemRarity1Txt;
    [SerializeField] private TextMeshProUGUI itemEffect1Txt;
    [SerializeField] private TextMeshProUGUI itemPrice1Txt;
    [SerializeField] private Button itemBuyBtn1;
    [SerializeField] private GameObject soldOutImage1;

    [Header("Item 2")]
    [SerializeField] private Image itemImage2;
    [SerializeField] private TextMeshProUGUI itemName2Txt;
    [SerializeField] private TextMeshProUGUI itemRarity2Txt;
    [SerializeField] private TextMeshProUGUI itemEffect2Txt;
    [SerializeField] private TextMeshProUGUI itemPrice2Txt;
    [SerializeField] private Button itemBuyBtn2;
    [SerializeField] private GameObject soldOutImage2;

    [Header("Item 3")]
    [SerializeField] private Image itemImage3;
    [SerializeField] private TextMeshProUGUI itemName3Txt;
    [SerializeField] private TextMeshProUGUI itemRarity3Txt;
    [SerializeField] private TextMeshProUGUI itemEffect3Txt;
    [SerializeField] private TextMeshProUGUI itemPrice3Txt;
    [SerializeField] private Button itemBuyBtn3;
    [SerializeField] private GameObject soldOutImage3;

    [Header("Reroll")]
    [SerializeField] private TextMeshProUGUI rerollCostTxt;
    [SerializeField] private Button rerollBtn;


    public void ToggleTraderUI(bool isActive)
    {
        traderUI.SetActive(isActive);
    }
    
    private void RerollItem1()
    {
        var consumableItemData = stageManager.GetRandomConsumableItemData();
        
        itemImage1.sprite = consumableItemData.GetIcon();
        itemName1Txt.text = consumableItemData.GetName();
        itemRarity1Txt.text = EnumManager.RarityToString(consumableItemData.GetRarity());
        if(consumableItemData.GetEffects().Length == 0) itemEffect1Txt.text += "None.";
        foreach (var itemEffect in consumableItemData.GetEffects())
        {
            itemEffect1Txt.text += $"{itemEffect.effectDescription}\n";
        }
        var itemPrice = stageManager.GetItemPrice(consumableItemData, consumableItemData.GetRarity());
        itemPrice1Txt.text = itemPrice.ToString();
        traderNpc.SetItemPrice(1, itemPrice);
        traderNpc.SetItemData(1, consumableItemData);
    }
    private void RerollItem2()
    {
        var weaponItemData = stageManager.GetRandomWeaponData();

        itemImage2.sprite = weaponItemData.GetIcon();
        itemName2Txt.text = weaponItemData.GetName();
        itemRarity2Txt.text = EnumManager.RarityToString(weaponItemData.GetRarity());
        if(weaponItemData.GetEffects().Length == 0) itemEffect2Txt.text += "None.";
        foreach (var itemEffect in weaponItemData.GetEffects())
        {
            itemEffect2Txt.text += $"{itemEffect.effectDescription}\n";
        }

        var itemPrice = stageManager.GetItemPrice(weaponItemData, weaponItemData.GetRarity());
        itemPrice2Txt.text = itemPrice.ToString();
        traderNpc.SetItemPrice(2, itemPrice);
        traderNpc.SetItemData(2, weaponItemData);
    }
    private void RerollItem3()
    {
        var equipmentItemData = stageManager.GetRandomEquipmentData();
        
        itemImage3.sprite = equipmentItemData.GetIcon();
        itemName3Txt.text = equipmentItemData.GetName();
        itemRarity3Txt.text = EnumManager.RarityToString(equipmentItemData.GetRarity());
        if (equipmentItemData.GetEffects().Length == 0) itemEffect3Txt.text += "None.";
        foreach (var itemEffect in equipmentItemData.GetEffects())
        {
            itemEffect3Txt.text += $"{itemEffect.effectDescription}\n";
        }
        var itemPrice = stageManager.GetItemPrice(equipmentItemData, equipmentItemData.GetRarity());
        itemPrice3Txt.text = itemPrice.ToString();
        traderNpc.SetItemPrice(3, itemPrice);
        traderNpc.SetItemData(3, equipmentItemData);
    }

    private void ItemReroll()
    {
        if (!_inventoryManager.UseMoney(traderNpc.GetRerollCost())) return;
        traderNpc.IncreaseRerollCost();
        rerollCostTxt.text = traderNpc.GetRerollCost().ToString();
        RerollItem1();
        RerollItem2();
        RerollItem3();
        soldOutImage1.SetActive(false);
        soldOutImage2.SetActive(false);
        soldOutImage3.SetActive(false);
    }
    
    private void HandleOnUpdateMoney(int money)
    {
        playerMoneyText.text = money.ToString();
        itemPrice1Txt.color = traderNpc.GetItemPrice(1) > money ? Color.red : Color.white;
        itemPrice2Txt.color = traderNpc.GetItemPrice(2) > money ? Color.red : Color.white;
        itemPrice3Txt.color = traderNpc.GetItemPrice(3) > money ? Color.red : Color.white;
        rerollCostTxt.color = traderNpc.GetRerollCost() > money ? Color.red : Color.white;
    }

    private void SpawnPurchasedItem(IItemData purchasedItem)
    {
        var position = traderNpc.transform.position;
        Instantiate(purchasedItem.GetItemPrefab(), position + Vector3.back*2f, Quaternion.identity);
    }

    private void PurchaseItem(int btnIdx)
    {
        switch (btnIdx)
        {
            case 1:
                if (_inventoryManager.UseMoney(traderNpc.GetItemPrice(1))) //금액이 충분하면 true리턴하고 금액계산
                {
                    soldOutImage1.SetActive(true);
                    SpawnPurchasedItem(traderNpc.GetItemData(1));
                }
                //else 구매금액 부족
                break;
            case 2:
                if (_inventoryManager.UseMoney(traderNpc.GetItemPrice(2)))
                {
                    soldOutImage2.SetActive(true);
                    SpawnPurchasedItem(traderNpc.GetItemData(2));
                }
                break;
            case 3:
                if (_inventoryManager.UseMoney(traderNpc.GetItemPrice(3)))
                {
                    soldOutImage3.SetActive(true);
                    SpawnPurchasedItem(traderNpc.GetItemData(3));
                }
                break;
        }
    }
    
    private void Awake()
    {
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _inventoryManager.OnUpdateMoneyAmount += HandleOnUpdateMoney;

        RerollItem1();
        RerollItem2();
        RerollItem3();
        
        rerollCostTxt.text = traderNpc.GetRerollCost().ToString();
        rerollBtn.onClick.AddListener(ItemReroll);

        itemBuyBtn1.onClick.AddListener(() => PurchaseItem(1));
        itemBuyBtn2.onClick.AddListener(() => PurchaseItem(2));
        itemBuyBtn3.onClick.AddListener(() => PurchaseItem(3));
    }
}
