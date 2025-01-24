using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using Scriptable_Objects;
using UnityEngine;


public class ItemDataWithID
{
    public ItemTypes ItemType;
    public IItemData ItemData;
    public GameObject ItemPrefab;
    public int ItemID = -1;
    public int ItemQuantity = 1;
}

public class InventoryManager : MonoBehaviour
{
    private int _weaponID = 1;//초기 ID 1부터
    private int _equipmentID = 1;
    
    private int _moneyAmount;
    
    public ItemDataWithID SelectedItem{private set; get; }
    public ItemDataWithID CurrentWeaponData { private set; get; }
    public ItemDataWithID CurrentEquipmentData { private set; get; }
    public ItemDataWithID ItemQuickSlot1Data { private set; get; }
    public ItemDataWithID ItemQuickSlot2Data { private set; get; }
    
    public List<ItemDataWithID> WeaponDataList{get; private set;} = new List<ItemDataWithID>();
    public List<ItemDataWithID> EquipmentDataList{get; private set;} = new List<ItemDataWithID>();
    public List<ItemDataWithID> ConsumableDataList { get; } = new List<ItemDataWithID>();
    
    public event Action<int> OnUpdateMoneyAmount;

    public int weaponInventoryCount { private set; get; }
    public int weaponInventoryMaxCount { private set; get; }

    public int equipmentInventoryCount { private set; get; }
    public int equipmentInventoryMaxCount { private set; get; }

    public int consumableInventoryCount { private set; get; }
    public int consumableInventoryMaxCount { private set; get; }
    //아이템별 현재 수량...
    public int consumableItemMaxQuantity { private set; get; }

    private const int DefaultMaxCount = 20;
    //inventory

    public void ResetInventory()
    {
        WeaponDataList.Clear();
        EquipmentDataList.Clear();
        ConsumableDataList.Clear();
        
        weaponInventoryCount = 0;
        equipmentInventoryCount = 0;
        consumableInventoryCount = 0;
        weaponInventoryMaxCount = DefaultMaxCount;
        equipmentInventoryMaxCount = DefaultMaxCount;
        consumableInventoryMaxCount = DefaultMaxCount;
        
        CurrentWeaponData = null;
        CurrentEquipmentData = null;
        ItemQuickSlot1Data = null;
        ItemQuickSlot2Data = null;
        SelectedItem = null;
        _weaponID = 1;
        _equipmentID = 1;
    }
    
    public void SetSelectedItem(ItemDataWithID itemDataWithID)
    {
        SelectedItem = itemDataWithID;
    }
    
    public void SetWeapon(ItemDataWithID itemDataWithID)//장착 무기 갱신(데이터, ID)
    {
        CurrentWeaponData = itemDataWithID;
    }

    public void SetEquipment(ItemDataWithID itemDataWithID)
    {
        CurrentEquipmentData = itemDataWithID;
    }

    public void SetQuickSlot1(ItemDataWithID itemDataWithID)
    {
        var consumableItemData = itemDataWithID != null ? 
            ConsumableDataList.FirstOrDefault(x => x.ItemData == itemDataWithID.ItemData) : null;
        
        ItemQuickSlot1Data = consumableItemData;
    }

    public void SetQuickSlot2(ItemDataWithID itemDataWithID)
    {
        var consumableItemData = itemDataWithID != null ?
            ConsumableDataList.FirstOrDefault(x => x.ItemData == itemDataWithID.ItemData) : null;
        ItemQuickSlot2Data = consumableItemData;
    }

    public void AddItemData(ItemDataWithID itemDataWithID)
    {
        var itemData = itemDataWithID.ItemData;
        switch (itemData)
        {
            case PlayerWeaponData:
                itemDataWithID.ItemID = _weaponID++;
                WeaponDataList.Add(itemDataWithID);//ID로 구분
                weaponInventoryCount = WeaponDataList.Count; //무기 개수 갱신 //null?
                WeaponDataList.Sort((a, b)//이름 오름차순 정렬
                    => string.Compare(a.ItemData.GetName(), b.ItemData.GetName(), StringComparison.Ordinal));
                break;
            case PlayerEquipmentData:
                itemDataWithID.ItemID = _equipmentID++;
                EquipmentDataList.Add(itemDataWithID);
                equipmentInventoryCount = EquipmentDataList.Count;
                EquipmentDataList.Sort( (a,b) 
                    => string.Compare(a.ItemData.GetName(), b.ItemData.GetName(), StringComparison.Ordinal));
                break;
            case ConsumableItemData:
                var existingItem = ConsumableDataList.FirstOrDefault(x => x.ItemData == itemDataWithID.ItemData);
                if (existingItem != null)//해당 아이템이 리스트에 있으면
                {
                    existingItem.ItemQuantity++;//수량증가
                }
                else
                {
                    ConsumableDataList.Add(itemDataWithID);//없으면 리스트에 추가
                    consumableInventoryCount = ConsumableDataList.Count;
                    ConsumableDataList.Sort( (a,b)
                        => string.Compare(a.ItemData.GetName(), b.ItemData.GetName(), StringComparison.Ordinal));
                }
                break;
            default:
                break;
        }
    }

    public void RemoveItemData(ItemDataWithID itemDataWithID)
    {
        switch (itemDataWithID.ItemData)
        {
            case PlayerWeaponData:
                WeaponDataList.Remove(itemDataWithID);
                weaponInventoryCount = WeaponDataList.Count;
                break;
            case PlayerEquipmentData:
                EquipmentDataList.Remove(itemDataWithID);
                equipmentInventoryCount = EquipmentDataList.Count;
                break;
            case ConsumableItemData:
                var index = ConsumableDataList.FindIndex(x => x == itemDataWithID);
                if (ConsumableDataList[index].ItemQuantity > 1)//수량이 1보다 크면 수량 감소
                {
                    ConsumableDataList[index].ItemQuantity--;
                }
                else
                {
                    ConsumableDataList.RemoveAt(index);//1이면 리스트에서 제거
                    consumableInventoryCount = ConsumableDataList.Count;//개수 갱신
                }
                break;
            default:
                break;
        }
    }
    
    public void AddMoney(int amount)//인벤토리 돈 추가
    {
        _moneyAmount += amount;
        OnUpdateMoneyAmount?.Invoke(_moneyAmount);//총 보유량 갱신 이벤트
    }

    private void Awake()
    {
        //base.Awake();
        
        weaponInventoryCount = 0;
        weaponInventoryMaxCount = DefaultMaxCount;
        
        equipmentInventoryCount = 0;
        equipmentInventoryMaxCount = DefaultMaxCount;
        
        consumableInventoryCount = 0;
        consumableInventoryMaxCount = DefaultMaxCount;

        consumableItemMaxQuantity = 10;//현재-모든 소비템 최대 보유 개수 통일

        _moneyAmount = 0;
        
    }
}
