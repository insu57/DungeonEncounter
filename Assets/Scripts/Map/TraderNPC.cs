using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderNPC : MonoBehaviour
{
   private PlayerManager _playerManager;
   private InventoryManager _inventoryManager;
   private float _distance;
   private StageManager _stageManager;
   
   [SerializeField] private GameObject traderUI;
   [SerializeField] private TextMeshProUGUI playerMoneyText;
   [Header("Item 1")]
   [SerializeField] private Image itemImage1;
   [SerializeField] private TextMeshProUGUI itemName1Txt;
   [SerializeField] private TextMeshProUGUI itemRarity1Txt;
   [SerializeField] private TextMeshProUGUI itemEffect1Txt;
   [SerializeField] private TextMeshProUGUI itemPrice1Txt;
   [SerializeField] private Button itemBuyBtn1;
   [Header("Item 2")]
   [SerializeField] private Image itemImage2;
   [SerializeField] private TextMeshProUGUI itemName2Txt;
   [SerializeField] private TextMeshProUGUI itemRarity2Txt;
   [SerializeField] private TextMeshProUGUI itemEffect2Txt;
   [SerializeField] private TextMeshProUGUI itemPrice2Txt;
   [SerializeField] private Button itemBuyBtn2;
   [Header("Item 3")]
   [SerializeField] private Image itemImage3;
   [SerializeField] private TextMeshProUGUI itemName3Txt;
   [SerializeField] private TextMeshProUGUI itemRarity3Txt;
   [SerializeField] private TextMeshProUGUI itemEffect3Txt;
   [SerializeField] private TextMeshProUGUI itemPrice3Txt;
   [SerializeField] private Button itemBuyBtn3;
   [Header("Reroll")]
   [SerializeField] private TextMeshProUGUI rerollCostTxt;
   [SerializeField] private Button rerollBtn;


   private void RerollItem()
   {
      //item1-Consumable
      var consumableItemData = _stageManager.GetRandomConsumableItemData();
      itemImage1.sprite = consumableItemData.GetIcon();
      itemName1Txt.text = consumableItemData.GetName();
      itemRarity1Txt.text = EnumManager.RarityToString(consumableItemData.GetRarity());
      foreach (var itemEffect in consumableItemData.GetEffects())
      {
         itemEffect1Txt.text += $"{itemEffect.effectDescription}\n";
      }
      //price?
   }
   
   private void HandleOnUpdateMoney(int money)
   {
      playerMoneyText.text = money.ToString();
   }
   
   private void Awake()
   {
      _playerManager = FindObjectOfType<PlayerManager>();
      _stageManager = FindObjectOfType<StageManager>();
      _inventoryManager = FindObjectOfType<InventoryManager>();
      _inventoryManager.OnUpdateMoneyAmount += HandleOnUpdateMoney;
      
      
   }

   private void Update()
   {
      _distance = Vector3.Distance(_playerManager.transform.position, transform.position);
      traderUI.SetActive(_distance < 1f);
   }
}
