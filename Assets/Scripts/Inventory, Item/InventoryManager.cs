using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using Scriptable_Objects;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int _weaponID = 1;//초기 ID 1부터
    private int _equipmentID = 1;

    public class ItemDataWithID
    {
        public ItemTypes ItemType;
        public IItemData ItemData;
        public GameObject ItemPrefab;
        public int ItemID = -1;
        public int ItemQuantity = 1;
    }

    public class SelectedItemData//현재 선택한 아이템
    {
        public ItemTypes ItemType; //아이템 타입
        public bool IsEquipped; //장착여부
        public ItemDataWithID ItemDataWithID;
    }
    
    private PlayerManager _playerManager;
    //[SerializeField] private ItemPrefabData itemPrefabData; //아이템데이터-프리팹 매핑
    //public ItemPrefabData ItemPrefabData => itemPrefabData;
    public SelectedItemData SelectedItem;
    
    private int _moneyAmount;
    //CurrentItem의 IsEquipped==false -> 현재 장착된 것이 없음.
    public ItemDataWithID EquippedWeaponData { private set; get; }
    public ItemDataWithID EquippedEquipmentData { private set; get; }
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
    
    //inventory

    public void SetWeapon(ItemDataWithID itemDataWithID)//장착 무기 갱신(데이터, ID)
    {
        EquippedWeaponData = itemDataWithID;
    }

    public void SetEquipment(ItemDataWithID itemDataWithID)
    {
        EquippedEquipmentData = itemDataWithID;
    }

    public void SetQuickSlot1(ItemDataWithID data)
    {
        ItemQuickSlot1Data = data;
    }

    public void SetQuickSlot2(ItemDataWithID data)
    {
        ItemQuickSlot2Data = data;
    }

    public void AddItemData(IItemData itemData, GameObject itemPrefab)
    {
        switch (itemData)
        {
            case PlayerWeaponData:
                WeaponDataList.Add(new ItemDataWithID()
                    { ItemData = itemData, ItemID = _weaponID++, ItemPrefab = itemPrefab});//ID로 구분
                weaponInventoryCount = WeaponDataList.Count; //무기 개수 갱신
                WeaponDataList.Sort((a, b)//이름 오름차순 정렬
                    => string.Compare(a.ItemData.GetName(), b.ItemData.GetName(), StringComparison.Ordinal));
                break;
            case PlayerEquipmentData:
                EquipmentDataList.Add(new ItemDataWithID 
                    { ItemData = itemData, ItemID = _equipmentID++, ItemPrefab = itemPrefab});
                equipmentInventoryCount = EquipmentDataList.Count;
                EquipmentDataList.Sort( (a,b) 
                    => string.Compare(a.ItemData.GetName(), b.ItemData.GetName(), StringComparison.Ordinal));
                break;
            case ConsumableItemData:
                var existingItem = ConsumableDataList.FirstOrDefault(x => x.ItemData == itemData);
                if (existingItem != null)//해당 아이템이 리스트에 있으면
                {
                    existingItem.ItemQuantity++;//수량증가
                }
                else
                {
                    ConsumableDataList.Add(new ItemDataWithID()
                        { ItemData = itemData, ItemPrefab = itemPrefab});//없으면 리스트에 추가
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
        weaponInventoryMaxCount = 20;
        
        equipmentInventoryCount = 0;
        equipmentInventoryMaxCount = 20;
        
        consumableInventoryCount = 0;
        consumableInventoryMaxCount = 20;

        consumableItemMaxQuantity = 10;//현재-모든 소비템 최대 보유 개수 통일

        _moneyAmount = 0;
        
        SelectedItem = new SelectedItemData();
        EquippedWeaponData = new ItemDataWithID()
        {
            ItemData = null,
            ItemID = -1,
            ItemType = ItemTypes.Weapon
        };
        
        EquippedEquipmentData = new ItemDataWithID()
        {
            ItemData = null,
            ItemID = -1,
            ItemType = ItemTypes.Equipment
        };
        ItemQuickSlot1Data = new ItemDataWithID()
        {
            ItemData = null,
            ItemQuantity = 0,
            ItemType = ItemTypes.Consumable
        };
        ItemQuickSlot2Data = new ItemDataWithID()
        {
            ItemData = null,
            ItemQuantity = 0,
            ItemType = ItemTypes.Consumable
        };
        //초기화(빈 상태)
    }
}
