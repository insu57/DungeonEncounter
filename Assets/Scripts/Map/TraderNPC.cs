using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TraderNpc : MonoBehaviour
{
   private PlayerManager _playerManager;
   //private InventoryManager _inventoryManager;
   private float _distance;
   [SerializeField] private GameObject pressF;
   [SerializeField] private StageManager stageManager;
   [SerializeField] private TraderNpcUI traderNpcUI;
   [SerializeField] private int rerollCost;
   private int _item1Price;
   private int _item2Price;
   private int _item3Price;
   private IItemData _item1Data;
   private IItemData _item2Data;
   private IItemData _item3Data;
   
   public int GetItemPrice(int itemIdx)//아이템 가격 리턴
   {
      return itemIdx switch
      {
         1 => _item1Price,
         2 => _item2Price,
         3 => _item3Price,
         _ => 0
      };
   }
   
   public IItemData GetItemData(int itemIdx)//아이템 데이터 리턴
   {
      return itemIdx switch
      {
         1 => _item1Data,
         2 => _item2Data,
         3 => _item3Data,
         _ => throw new ArgumentOutOfRangeException(nameof(itemIdx), itemIdx,$"Invalid ItemIdx...{itemIdx}. (Idx: 1~3)")
         //예외처리
      };
   }

   public void SetItemData(int itemIdx)//아이템데이터 설정
   {
      switch (itemIdx)
      {
         case 1:
            _item1Data = stageManager.GetRandomConsumableItemData();
            _item1Price = stageManager.GetItemPrice(_item1Data);
            break;
         case 2:
            _item2Data = stageManager.GetRandomWeaponData(); 
            _item2Price = stageManager.GetItemPrice(_item2Data);
            break;
         case 3:
            _item3Data = stageManager.GetRandomEquipmentData();
            _item3Price = stageManager.GetItemPrice(_item3Data);
            break;
      }
   }

   public int GetRerollCost() //리롤 비용 리턴
   {
      return rerollCost;
   }
   
   public void IncreaseRerollCost() //한 번 리롤마다 비용 1.5배 상승
   {
      rerollCost = Convert.ToInt32(rerollCost * 1.5);
   }
   
   public void SpawnPurchasedItem(int itemIdx) //구매아이템 스폰
   {
      Instantiate(GetItemData(itemIdx).GetItemPrefab(), transform.position + Vector3.back*2f, Quaternion.identity);
   }
   
   private void Awake()
   {
      _playerManager = FindObjectOfType<PlayerManager>();
     
   }

   private void Update()
   {
      _distance = Vector3.Distance(_playerManager.transform.position, transform.position);
      if (Input.GetKeyDown(KeyCode.F) && _distance <= 1.5f)
      {
         traderNpcUI.ToggleTraderUI(true);//거래UI
      }

      if (_distance > 1.5f)
      {
         traderNpcUI.ToggleTraderUI(false);
      }
      
      
      pressF.SetActive(_distance <= 1.5f);
   }
}
