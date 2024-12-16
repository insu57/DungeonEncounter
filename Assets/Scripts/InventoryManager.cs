using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using Scriptable_Objects;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int _weaponID = 1;
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

    public class SelectedItemData
    {
        public ItemTypes ItemType; //아이템 타입
        public bool IsEquipped; //장착여부
        public WeaponDataWithID Weapon; //타입에 따라 불러올 데이터
        public EquipmentDataWithID Equipment;
        public ConsumableDataWithQuantity Consumable;
    }
    
    private PlayerManager _playerManager;
    [SerializeField] private ItemPrefabData itemPrefabData;//아이템데이터-프리팹 매핑
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

    //public Action 
    //public Event changeWeapon;
    //public Event changeEquipment;
    public event Action<int> OnUpdateMoneyAmount; 
    public Event UpdateInventory;

    public int weaponInventoryCount { private set; get; }
    public int weaponInventoryMaxCount { private set; get; }

    public int equipmentInventoryCount { private set; get; }
    public int equipmentInventoryMaxCount { private set; get; }

    public int consumableInventoryCount { private set; get; }
    public int consumableInventoryMaxCount { private set; get; }
    //아이템별 현재 수량...
    public int consumableItemMaxQuantity { private set; get; }
    
    //inventory

    public void SetWeapon(PlayerWeaponData data, int weaponID)
    {
        EquippedWeaponData.ItemData = data;
        EquippedWeaponData.WeaponID = weaponID;
    }

    public void SetEquipment(PlayerEquipmentData data, int index)
    {
        EquippedEquipmentData.ItemData = data;
        EquippedEquipmentData.EquipmentID = index;
    }

    public void SetQuickSlot1(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot1 = data;
    }

    public void SetQuickSlot2(ConsumableDataWithQuantity data)
    {
        ItemQuickSlot2 = data;
    }

    public void AddWeaponData(PlayerWeaponData data)
    {
        WeaponDataList.Add(new WeaponDataWithID { ItemData = data, WeaponID = _weaponID++ });
        weaponInventoryCount = WeaponDataList.Count;
        WeaponDataList.Sort((a, b)
            => string.Compare(a.ItemData.WeaponName, b.ItemData.WeaponName, StringComparison.Ordinal));
    }

    public void RemoveWeaponData(WeaponDataWithID weapon, Transform playerTransform)
    {
        WeaponDataList.Remove(weapon);
        weaponInventoryCount = WeaponDataList.Count;
        //오브젝트 풀링?
        Instantiate(itemPrefabData.GetWeaponPrefab(weapon.ItemData),
            playerTransform.position + Vector3.back,Quaternion.identity);
    }
    
    public void AddEquipmentData(PlayerEquipmentData data)
    {
        EquipmentDataList.Add(new EquipmentDataWithID { ItemData = data, EquipmentID = _equipmentID++ });
        equipmentInventoryCount = EquipmentDataList.Count;
        EquipmentDataList.Sort( (a,b) 
            => string.Compare(a.ItemData.EquipmentName, b.ItemData.EquipmentName, StringComparison.Ordinal));
    }

    public void RemoveEquipmentData(EquipmentDataWithID equipment, Transform playerTransform)
    {
        EquipmentDataList.Remove(equipment);
        equipmentInventoryCount = EquipmentDataList.Count;
        Instantiate(itemPrefabData.GetEquipmentPrefab(equipment.ItemData),
            playerTransform.position + Vector3.back,Quaternion.identity);
    }
    
    public void AddConsumableData(ConsumableItemData data)
    {
        var existingItem = ConsumableDataList.FirstOrDefault(x => x.ItemData == data);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            ConsumableDataList.Add(new ConsumableDataWithQuantity { ItemData = data, Quantity = 1 });
            consumableInventoryCount = ConsumableDataList.Count;
            ConsumableDataList.Sort( (a,b)
                => string.Compare(a.ItemData.ItemName, b.ItemData.ItemName, StringComparison.Ordinal));
        }
    }

    public void RemoveConsumableData(ConsumableDataWithQuantity consumable, Transform playerTransform)
    {
        var index = ConsumableDataList.FindIndex(x => x == consumable);
        if (ConsumableDataList[index].Quantity > 1)
        {
            ConsumableDataList[index].Quantity--;
        }
        else
        {
            ConsumableDataList.RemoveAt(index);
            consumableInventoryCount = ConsumableDataList.Count;
        }
        Instantiate(itemPrefabData.GetConsumablePrefab(consumable.ItemData),
            playerTransform.position + Vector3.back,Quaternion.identity);
    }
    
    public void AddMoney(int amount)
    {
        _moneyAmount += amount;
        OnUpdateMoneyAmount?.Invoke(_moneyAmount);
    }
    
    private void Awake()
    {
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
