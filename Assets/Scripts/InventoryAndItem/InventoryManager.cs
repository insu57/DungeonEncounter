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
    public class WeaponDataWithID
    {
        public PlayerWeaponData ItemData;
        public int WeaponID;
    }

    public class EquipmentDataWithID
    {
        public PlayerEquipmentData ItemData;
        public int EquipmentID;
    }
    public class ConsumableDataWithQuantity
    {
        public ConsumableItemData ItemData;
        public int Quantity;
    }

    public class SelectedItemData//현재 선택한 아이템
    {
        public ItemTypes ItemType; //아이템 타입
        public bool IsEquipped; //장착여부
        public WeaponDataWithID Weapon; //타입에 따라 불러올 데이터
        public EquipmentDataWithID Equipment;
        public ConsumableDataWithQuantity Consumable;
    }
    
    private PlayerManager _playerManager;
    [SerializeField] private ItemPrefabData itemPrefabData; //아이템데이터-프리팹 매핑
    public ItemPrefabData ItemPrefabData => itemPrefabData;
    public SelectedItemData SelectedItem;
    
    private int _moneyAmount;
    //CurrentItem의 IsEquipped==false -> 현재 장착된 것이 없음.
    public WeaponDataWithID EquippedWeaponData { private set; get; }
    public EquipmentDataWithID EquippedEquipmentData { private set; get; }
    public ConsumableDataWithQuantity ItemQuickSlot1 { private set; get; }
    public ConsumableDataWithQuantity ItemQuickSlot2 { private set; get; }
    
    public List<WeaponDataWithID> WeaponDataList { get; } = new List<WeaponDataWithID>();
    public List<EquipmentDataWithID> EquipmentDataList { get; } = new List<EquipmentDataWithID>();
    public List<ConsumableDataWithQuantity> ConsumableDataList { get; } = new List<ConsumableDataWithQuantity>();
    
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

    public void SetWeapon(PlayerWeaponData data, int weaponID)//장착 무기 갱신(데이터, ID)
    {
        EquippedWeaponData.ItemData = data;
        EquippedWeaponData.WeaponID = weaponID;
    }

    public void SetEquipment(PlayerEquipmentData data, int equipmentID)
    {
        EquippedEquipmentData.ItemData = data;
        EquippedEquipmentData.EquipmentID = equipmentID;
    }

    public void SetQuickSlot1(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot1 = data;
    }

    public void SetQuickSlot2(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot2 = data;
    }

    public void AddWeaponData(PlayerWeaponData data)//무기 아이템 데이터 추가
    {
        WeaponDataList.Add(new WeaponDataWithID { ItemData = data, WeaponID = _weaponID++ });//ID로 구분
        weaponInventoryCount = WeaponDataList.Count; //무기 개수 갱신
        WeaponDataList.Sort((a, b)//이름 오름차순 정렬
            => string.Compare(a.ItemData.WeaponName, b.ItemData.WeaponName, StringComparison.Ordinal));
    }

    public void RemoveWeaponData(WeaponDataWithID weapon)
    {
        WeaponDataList.Remove(weapon);
        weaponInventoryCount = WeaponDataList.Count;//개수 갱신
        
    }
    
    public void AddEquipmentData(PlayerEquipmentData data)
    {
        EquipmentDataList.Add(new EquipmentDataWithID { ItemData = data, EquipmentID = _equipmentID++ });
        equipmentInventoryCount = EquipmentDataList.Count;
        EquipmentDataList.Sort( (a,b) 
            => string.Compare(a.ItemData.EquipmentName, b.ItemData.EquipmentName, StringComparison.Ordinal));
    }

    public void RemoveEquipmentData(EquipmentDataWithID equipment)
    {
        EquipmentDataList.Remove(equipment);
        equipmentInventoryCount = EquipmentDataList.Count;
    }
    
    public void AddConsumableData(ConsumableItemData data)
    {
        var existingItem = ConsumableDataList.FirstOrDefault(x => x.ItemData == data);
        if (existingItem != null)//해당 아이템이 리스트에 있으면
        {
            existingItem.Quantity++;//수량증가
        }
        else
        {
            ConsumableDataList.Add(new ConsumableDataWithQuantity { ItemData = data, Quantity = 1 });//없으면 리스트에 추가
            consumableInventoryCount = ConsumableDataList.Count;
            ConsumableDataList.Sort( (a,b)
                => string.Compare(a.ItemData.ItemName, b.ItemData.ItemName, StringComparison.Ordinal));
        }
    }

    public void RemoveConsumableData(ConsumableDataWithQuantity consumable)
    {
        var index = ConsumableDataList.FindIndex(x => x == consumable);
        if (ConsumableDataList[index].Quantity > 1)//수량이 1보다 크면 수량 감소
        {
            ConsumableDataList[index].Quantity--;
        }
        else
        {
            ConsumableDataList.RemoveAt(index);//1이면 리스트에서 제거
            consumableInventoryCount = ConsumableDataList.Count;//개수 갱신
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
        EquippedWeaponData = new WeaponDataWithID
        {
            ItemData = null,
            WeaponID = -1
        };
        
        EquippedEquipmentData = new EquipmentDataWithID()
        {
            ItemData = null,
            EquipmentID = -1
        };
        ItemQuickSlot1 = new ConsumableDataWithQuantity()
        {
            ItemData = null,
            Quantity = 0
        };
        ItemQuickSlot2 = new ConsumableDataWithQuantity()
        {
            ItemData = null,
            Quantity = 0
        };
        //초기화(빈 상태)
    }
}
