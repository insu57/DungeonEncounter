using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderNpcUI : MonoBehaviour
{
    //private InventoryManager _inventoryManager;
    //[SerializeField] private StageManager stageManager;
    //[SerializeField] private TraderNpc traderNpc;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject traderUI;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private Button closeButton;
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
    
    public event Action OnOnRerollBtnClicked;
    public event Action<int> OnItemPurchasedBtnClicked;
    
    public void ToggleTraderUI(bool isActive)
    {
        if (traderUI.activeSelf == isActive) return;
        traderUI.SetActive(isActive);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.InventoryOpenSfx);
    }
    
    public void RerollItem1(ConsumableItemData consumableItemData, int itemPrice)
    {
        soldOutImage1.SetActive(false);
        //아이템 
        itemImage1.sprite = consumableItemData.GetIcon(); //아이콘
        itemName1Txt.text = consumableItemData.GetName(); //이름
        itemRarity1Txt.text = EnumManager.RarityToString(consumableItemData.GetRarity()); //레어도
        if(consumableItemData.GetEffects().Length == 0) itemEffect1Txt.text += "None."; //효과 없으면 NONE.
        foreach (var itemEffect in consumableItemData.GetEffects())
        {
            itemEffect1Txt.text += $"\n{itemEffect.effectDescription}"; //있으면 효과 설명 추가
        }
        itemPrice1Txt.text = itemPrice.ToString(); //가격 표시
    }

    public void RerollItem2(PlayerWeaponData weaponItemData, int itemPrice)
    {
        soldOutImage2.SetActive(false);
        itemImage2.sprite = weaponItemData.GetIcon();
        itemName2Txt.text = weaponItemData.GetName();
        itemRarity2Txt.text = EnumManager.RarityToString(weaponItemData.GetRarity());
        if(weaponItemData.GetEffects().Length == 0) itemEffect2Txt.text += "None.";
        foreach (var itemEffect in weaponItemData.GetEffects())
        {
            itemEffect2Txt.text += $"{itemEffect.effectDescription}\n";
        }
        itemPrice2Txt.text = itemPrice.ToString();
    }

    public void RerollItem3(PlayerEquipmentData equipmentItemData, int itemPrice)
    {
        soldOutImage3.SetActive(false);
        itemImage3.sprite = equipmentItemData.GetIcon();
        itemName3Txt.text = equipmentItemData.GetName();
        itemRarity3Txt.text = EnumManager.RarityToString(equipmentItemData.GetRarity());
        if (equipmentItemData.GetEffects().Length == 0) itemEffect3Txt.text += "None.";
        foreach (var itemEffect in equipmentItemData.GetEffects())
        {
            itemEffect3Txt.text += $"{itemEffect.effectDescription}\n";
        }
        itemPrice3Txt.text = itemPrice.ToString();
    }

    public void UpdateCurrentMoney(int currentMoney)
    {
        playerMoneyText.text = currentMoney.ToString();
    }
    
    public void UpdateRerollCostTxt(int rerollCost, Color color)
    {
        rerollCostTxt.text = rerollCost.ToString();
    }

    public void UpdateItemPriceTxtColor(int itemIdx, Color color)
    {
        switch (itemIdx)
        {
            case 1:
                itemPrice1Txt.color = color;
                break;
            case 2:
                itemPrice2Txt.color = color;
                break;
            case 3:
                itemPrice3Txt.color = color;
                break;
        }
    }

    public void ItemSoldOut(int itemIdx)
    {
        switch (itemIdx)
        {
            case 1:
                soldOutImage1.SetActive(true);
                break;
            case 2:
                soldOutImage2.SetActive(true);
                break;
            case 3:
                soldOutImage3.SetActive(true);
                break;
        }
    }
    
    private void Awake()
    {
        closeButton.onClick.AddListener(() => ToggleTraderUI(false));//닫기
        rerollBtn.onClick.AddListener(() =>
        { 
            OnOnRerollBtnClicked?.Invoke();
        });
        //버튼별 콜백함수 등록
        itemBuyBtn1.onClick.AddListener( () =>
        {
            OnItemPurchasedBtnClicked?.Invoke(1);
        }); 
        itemBuyBtn2.onClick.AddListener(() =>
        {
            OnItemPurchasedBtnClicked?.Invoke(2);
        });
        itemBuyBtn3.onClick.AddListener(() =>
        {
            OnItemPurchasedBtnClicked?.Invoke(3);
        });
    }
}
