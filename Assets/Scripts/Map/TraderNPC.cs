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
   private InventoryManager _inventoryManager;
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
   
   public int GetItemPrice(int itemIdx)
   {
      return itemIdx switch
      {
         1 => _item1Price,
         2 => _item2Price,
         3 => _item3Price,
         _ => 0
      };
   }

   public void SetItemPrice(int itemIdx, int itemPrice)
   {
      switch (itemIdx)
      {
         case 1: _item1Price = itemPrice; break;
         case 2: _item2Price = itemPrice; break;
         case 3: _item3Price = itemPrice; break;
      }
   }

   public IItemData GetItemData(int itemIdx)
   {
      return itemIdx switch
      {
         1 => _item1Data,
         2 => _item2Data,
         3 => _item3Data,
         _ => throw new ArgumentOutOfRangeException(nameof(itemIdx), itemIdx,$"Invalid ItemIdx...{itemIdx}. (Idx: 1~3)")
      };
   }

   public void SetItemData(int itemIdx, IItemData itemData)
   {
      switch (itemIdx)
      {
         case 1: _item1Data = itemData; break;
         case 2: _item2Data = itemData; break;
         case 3: _item3Data = itemData; break;
      }
   }

   public int GetRerollCost()
   {
      return rerollCost;
   }
   
   public void IncreaseRerollCost()
   {
      rerollCost = Convert.ToInt32(rerollCost * 1.5);
   }
   
   private void Awake()
   {
      _playerManager = FindObjectOfType<PlayerManager>();
   }

   private void Update()
   {
      _distance = Vector3.Distance(_playerManager.transform.position, transform.position);
      if (Input.GetKeyDown(KeyCode.F))
      {
         traderNpcUI.ToggleTraderUI(_distance <= 1.5f);
      }
      pressF.SetActive(_distance <= 1.5f);
   }
}
