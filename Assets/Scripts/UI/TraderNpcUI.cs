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


    public void ToggleTraderUI(bool isActive)
    {
        traderUI.SetActive(isActive);
    }
    
    private void RerollItem1()
    {
        //아이템 리롤
        var consumableItemData = stageManager.GetRandomConsumableItemData(); //랜덤 아이템 데이터 받아오기
        
        itemImage1.sprite = consumableItemData.GetIcon(); //아이콘
        itemName1Txt.text = consumableItemData.GetName(); //이름
        itemRarity1Txt.text = EnumManager.RarityToString(consumableItemData.GetRarity()); //레어도
        if(consumableItemData.GetEffects().Length == 0) itemEffect1Txt.text += "None."; //효과 없으면 NONE.
        foreach (var itemEffect in consumableItemData.GetEffects())
        {
            itemEffect1Txt.text += $"\n{itemEffect.effectDescription}"; //있으면 효과 설명 추가
        }
        var itemPrice = stageManager.GetItemPrice(consumableItemData, consumableItemData.GetRarity());
        //종류, 레어도에 따라 가격 가져오기
        itemPrice1Txt.text = itemPrice.ToString(); //가격 표시
        traderNpc.SetItemPrice(1, itemPrice); //가격 설정
        traderNpc.SetItemData(1, consumableItemData); //데이터 설정
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
        //전체 아이템 리롤
        if (!_inventoryManager.UseMoney(traderNpc.GetRerollCost())) return; //가격 부족하면 실패
        traderNpc.IncreaseRerollCost(); //비용 증가
        rerollCostTxt.text = traderNpc.GetRerollCost().ToString(); //비용 표시 업데이트
        RerollItem1(); //1~3 아이템 리롤
        RerollItem2();
        RerollItem3();
        soldOutImage1.SetActive(false); //판매완료 이미지 숨김
        soldOutImage2.SetActive(false);
        soldOutImage3.SetActive(false);
    }
    
    private void HandleOnUpdateMoney(int money)
    {
        playerMoneyText.text = money.ToString(); //가격 변동 시 업데이트
        itemPrice1Txt.color = traderNpc.GetItemPrice(1) > money ? Color.red : Color.white;
        itemPrice2Txt.color = traderNpc.GetItemPrice(2) > money ? Color.red : Color.white;
        itemPrice3Txt.color = traderNpc.GetItemPrice(3) > money ? Color.red : Color.white;
        rerollCostTxt.color = traderNpc.GetRerollCost() > money ? Color.red : Color.white;
    }

    private void SpawnPurchasedItem(IItemData purchasedItem) //구매아이템 스폰
    {
        var position = traderNpc.transform.position;
        Instantiate(purchasedItem.GetItemPrefab(), position + Vector3.back*2f, Quaternion.identity);
    }

    private void PurchaseItem(int btnIdx)
    {
        //아이템 구매
        switch (btnIdx)
        {
            case 1:
                if (_inventoryManager.UseMoney(traderNpc.GetItemPrice(1))) //금액이 충분하면 true리턴하고 금액계산
                {
                    soldOutImage1.SetActive(true); //판매완료 이미지 활성
                    SpawnPurchasedItem(traderNpc.GetItemData(1)); // 구매 아이템 스폰
                }
                //else 구매금액 부족시 처리?
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
        
        closeButton.onClick.AddListener(() => ToggleTraderUI(false));//닫기
        //초기화
        RerollItem1();
        RerollItem2();
        RerollItem3();
        rerollCostTxt.text = traderNpc.GetRerollCost().ToString();
        rerollBtn.onClick.AddListener(ItemReroll);//버튼별 콜백함수 등록
        itemBuyBtn1.onClick.AddListener(() => PurchaseItem(1)); 
        itemBuyBtn2.onClick.AddListener(() => PurchaseItem(2));
        itemBuyBtn3.onClick.AddListener(() => PurchaseItem(3));
    }
}
