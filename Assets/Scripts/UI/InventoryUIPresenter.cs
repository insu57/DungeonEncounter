using Player;
using Scriptable_Objects;
using UI;
using UnityEngine;

public class InventoryUIPresenter
{
    private readonly PlayerManager _playerManager;
    private readonly InventoryManager _inventoryManager;
    private readonly InventoryUIView _inventoryUIView;

    public InventoryUIPresenter(PlayerManager playerManager, InventoryManager inventoryManager,
        InventoryUIView inventoryUIView)
    {
        //Refactoring?
        _playerManager = playerManager;
        _inventoryManager = inventoryManager;
        _inventoryUIView = inventoryUIView;

        _inventoryUIView.OnInventoryOpen += HandleOnInventoryOpen;
        _inventoryUIView.OnCurrentWeapon += HandleShowCurrentWeapon;
        _inventoryUIView.OnCurrentEquipment += HandleShowCurrentEquipment;
        _inventoryUIView.OnQuickSlot1 += HandleShowCurrentQuick1;
        _inventoryUIView.OnQuickSlot2 += HandleShowCurrentQuick2;
        _inventoryUIView.OnSelectInventorySlot += HandleSelectInventorySlot;

        _playerManager.OnGetMoney += HandleAddMoney;
        _playerManager.OnGetItem += HandleAddItem;
        _playerManager.OnUseItemQuickSlot += HandleOnUseItemQuickSlot;
        _inventoryManager.OnUpdateMoneyAmount += HandleUpdateTotalMoney;
     
        _inventoryUIView.OnEquipButton += HandleOnEquipButton;
        _inventoryUIView.OnDropButton += HandleOnDropButton;
        _inventoryUIView.OnSetQuickSlot += HandleOnSetQuickSlot;
        
        
        //init
        _inventoryManager.AddWeaponData(_playerManager.WeaponData);
        _inventoryManager.SetWeapon(_playerManager.WeaponData, 1);
        _inventoryUIView.UpdateEquippedWeapon(_playerManager.WeaponData.Icon);
        int maxCount = _inventoryManager.weaponInventoryMaxCount;  //생성은 초기화, 최대칸 증가 시에만
        for (int i = 0; i < maxCount; i++)
        {
            _inventoryUIView.InitInventory();
        }

    }
    
    private void HandleShowCurrentWeapon() //현재 무기 데이터 표시
    {
        PlayerWeaponData data = _inventoryManager.EquippedWeaponData.ItemData;
        if (data != null)
        {
            _inventoryUIView.SelectedWeapon(data);
            _inventoryUIView.SetEquipBtnInteractable(false);//무기는 무조건 하나 장착해야함.(장착해제x)
            _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍 불가
        
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
            _inventoryManager.SelectedItem.Weapon = _inventoryManager.EquippedWeaponData;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
    }
    
    private void HandleShowCurrentEquipment()
    {
        PlayerEquipmentData data = _inventoryManager.EquippedEquipmentData.ItemData;
        if (data != null)
        {
            _inventoryUIView.SelectedEquipment(data);
            _inventoryUIView.ToggleItemEquipBtn(true);//장착버튼 토글.
            _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍 불가
            
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
            _inventoryManager.SelectedItem.Equipment = _inventoryManager.EquippedEquipmentData;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }
    private void HandleShowCurrentQuick1()
    {
        InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ItemQuickSlot1;
        if (item.ItemData != null)
        {
            _inventoryUIView.SelectedConsumable(item);
            _inventoryUIView.ToggleQuick1Btn(true);
            _inventoryUIView.ToggleQuick2Btn(_inventoryManager.ItemQuickSlot2 != null 
                                             && item.ItemData == _inventoryManager.ItemQuickSlot2.ItemData);
            //다른 퀵슬롯에 장착된 상태인지 확인
            _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍불가
            
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot1;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
        
    }

    private void HandleShowCurrentQuick2()
    {
        InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ItemQuickSlot2;
        if (item.ItemData != null)
        {
            _inventoryUIView.SelectedConsumable(item);
            _inventoryUIView.ToggleQuick1Btn(_inventoryManager.ItemQuickSlot1 != null 
                                             && item.ItemData == _inventoryManager.ItemQuickSlot1.ItemData);
            _inventoryUIView.ToggleQuick2Btn(true);
            _inventoryUIView.SetDropBtnInteractable(false);    
            
            _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
            _inventoryManager.SelectedItem.Consumable = _inventoryManager.ItemQuickSlot2;
            _inventoryManager.SelectedItem.IsEquipped = true;
        }
    }

    private void HandleSelectInventorySlot(ItemTypes type, int index) //선택한 인벤토리 슬롯 아이템 데이터 표시
    {
        switch (type)//아이템 타입 따라
        {
            case ItemTypes.Weapon:
                if (index < _inventoryManager.weaponInventoryCount)
                {
                    InventoryManager.WeaponDataWithID item = _inventoryManager.WeaponDataList[index];//선택한 아이템 데이터
                    bool isEquipped = item.WeaponID == _inventoryManager.EquippedWeaponData.WeaponID;//장착한 아이템과 인덱스 비교
                    
                    _inventoryUIView.SelectedWeapon(item.ItemData);//데이터 표시
                    if (item.ItemData.IsDefaultWeapon || isEquipped) //장착한 무기나 기본 무기는 드랍불가
                    {
                        _inventoryUIView.SetDropBtnInteractable(false);
                    }
                    else
                    {
                        _inventoryUIView.SetDropBtnInteractable(true);
                    }
                    _inventoryUIView.ToggleItemEquipBtn(isEquipped);
                    _inventoryUIView.SetEquipBtnInteractable(!isEquipped);//장착한 무기는 장착버튼 비활성
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Weapon;
                    _inventoryManager.SelectedItem.Weapon = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Equipment:
                if (index < _inventoryManager.equipmentInventoryCount)
                {
                    InventoryManager.EquipmentDataWithID item = _inventoryManager.EquipmentDataList[index];
                    bool isEquipped = item.EquipmentID == _inventoryManager.EquippedEquipmentData.EquipmentID; 
                    
                    _inventoryUIView.SelectedEquipment(item.ItemData);
                    _inventoryUIView.ToggleItemEquipBtn(isEquipped);
                    _inventoryUIView.SetDropBtnInteractable(!isEquipped);
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Equipment;
                    _inventoryManager.SelectedItem.Equipment = item;
                    _inventoryManager.SelectedItem.IsEquipped = isEquipped;
                }
                break;
            case ItemTypes.Consumable:
                if (index < _inventoryManager.consumableInventoryCount)//소모템은 인덱스없음(중복가능해서 현재는 없음)
                {
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.ConsumableDataList[index];
                    _inventoryUIView.SelectedConsumable(item);
                    //퀵슬롯 설정
                    bool isQuick1 = _inventoryManager.ItemQuickSlot1 != null &&//null확인(빈 상태인지)
                                    item.ItemData == _inventoryManager.ItemQuickSlot1.ItemData;//퀵슬롯 장착 확인
                    bool isQuick2 = _inventoryManager.ItemQuickSlot2 != null &&
                                    item.ItemData == _inventoryManager.ItemQuickSlot2.ItemData;
                    
                    _inventoryUIView.ToggleQuick1Btn(isQuick1);
                    _inventoryUIView.ToggleQuick2Btn(isQuick2);
                    _inventoryUIView.SetDropBtnInteractable(!(isQuick1||isQuick2)); //수량따라 작동하게 수정필요
                    
                    _inventoryManager.SelectedItem.ItemType = ItemTypes.Consumable;
                    _inventoryManager.SelectedItem.Consumable = item;
                    _inventoryManager.SelectedItem.IsEquipped = isQuick1 || isQuick2;
                }
                break;
            default:
                break;
        }
    }
    
    private void HandleOnInventoryOpen(ItemTypes type) //인벤토리 열기
    {
        int count, maxCount;
        switch (type)//타입에 따라
        {
            case ItemTypes.Weapon:
                count = _inventoryManager.weaponInventoryCount;//현재 개수
                maxCount = _inventoryManager.weaponInventoryMaxCount;//최대 개수
                _inventoryUIView.UpdateInventoryCount(count, maxCount);//개수 표시
                _inventoryUIView.ClearInventoryIcon(maxCount);//초기화
                int index = 0;
                foreach (var weaponData in _inventoryManager.WeaponDataList)//아이콘 표시
                {
                    _inventoryUIView.UpdateInventoryIcon(index, weaponData.ItemData.Icon);
                    index++;
                }
                
                break;
            case ItemTypes.Equipment:
                count = _inventoryManager.equipmentInventoryCount;
                maxCount = _inventoryManager.equipmentInventoryMaxCount;
                _inventoryUIView.UpdateInventoryCount(count, maxCount);
                _inventoryUIView.ClearInventoryIcon(maxCount);
                index = 0;
                foreach (var equipmentData in _inventoryManager.EquipmentDataList)
                {
                    _inventoryUIView.UpdateInventoryIcon(index, equipmentData.ItemData.Icon);
                    index++;
                }
                break;
            case ItemTypes.Consumable:
                count = _inventoryManager.consumableInventoryCount;
                maxCount = _inventoryManager.consumableInventoryMaxCount;
                _inventoryUIView.UpdateInventoryCount(count, maxCount);
                _inventoryUIView.ClearInventoryIcon(maxCount);
                index = 0;
                foreach (var consumableItem in _inventoryManager.ConsumableDataList)
                {
                    int quantity = consumableItem.Quantity;//수량도 표시
                    _inventoryUIView.UpdateInventoryIconWithQuantity(index, consumableItem.ItemData.Icon, quantity);
                    index++;
                }
                break;
        }
        
        
    }

    private void HandleAddMoney(int money)//돈 획득
    {
        _inventoryManager.AddMoney(money);
    }

    private void HandleAddItem(GameObject item)//아이템 획득
    {
        switch (item.layer)
        {
            case (int)ItemLayers.Weapon:
            {
                PlayerWeaponData weaponData = item.GetComponent<PlayerWeapon>().WeaponData;
                _inventoryManager.AddWeaponData(weaponData);
                break;
            }
            case (int)ItemLayers.Equipment:
            {
                PlayerEquipmentData equipmentData = item.GetComponent<PlayerEquipment>().Data;
                _inventoryManager.AddEquipmentData(equipmentData);
                break;
            }
            case (int)ItemLayers.Consumable:
            {
                ConsumableItemData consumableData = item.GetComponent<ConsumableItem>().Data;
                _inventoryManager.AddConsumableData(consumableData);
                break;
            }
        }
    }

    private void HandleUpdateTotalMoney(int moneyAmount)//현재 보유한 돈 표시
    {
        _inventoryUIView.UpdateMoney(moneyAmount);
        
    }

    private void HandleOnEquipButton()//장착버튼 ->스탯 반영
    {
        ItemTypes itemType = _inventoryManager.SelectedItem.ItemType;
        switch (itemType)//아이템 타입 따라
        {
            case ItemTypes.Weapon: //항상 장착만(장착 해제 불가, 무기 하나는 들고있어야함)
            {
                InventoryManager.WeaponDataWithID dataWithID = _inventoryManager.SelectedItem.Weapon;
                _inventoryManager.SetWeapon(dataWithID.ItemData,dataWithID.WeaponID);
                _playerManager.PlayerEquipWeapon(dataWithID.ItemData);//player equip weapon
                
                _inventoryUIView.UpdateEquippedWeapon(dataWithID.ItemData.Icon);//현재 장착 무기 아이콘
                _inventoryUIView.SetEquipBtnInteractable(false);
                _inventoryUIView.SetDropBtnInteractable(false);//장착, 드랍 상호작용 비활성
                break;
            }
            case ItemTypes.Equipment:
            {
                if (_inventoryManager.SelectedItem.IsEquipped)//장착된 아이템인지 확인
                {
                    _inventoryManager.SelectedItem.IsEquipped = false;  //선택 장비 장착 해제
                    _inventoryManager.SetEquipment(null, -1); //장착 장비 초기화
                    _playerManager.PlayerEquipEquipment(null);
                    
                    _inventoryUIView.ToggleItemEquipBtn(false); //장착버튼 토글(장착/해제 텍스트)
                    _inventoryUIView.SetDropBtnInteractable(true); //드랍 가능
                    _inventoryUIView.ClearEquippedEquipment(); //PlayerStatus에서 비활성
                }
                else
                {
                    InventoryManager.EquipmentDataWithID dataWithID = _inventoryManager.SelectedItem.Equipment;
                    _inventoryManager.SetEquipment(dataWithID.ItemData,dataWithID.EquipmentID);
                    _inventoryManager.SelectedItem.IsEquipped = true;
                    _playerManager.PlayerEquipEquipment(dataWithID.ItemData);
                    
                    _inventoryUIView.UpdateEquippedEquipment(dataWithID.ItemData.Icon);//아이콘 업데이트
                    _inventoryUIView.ToggleItemEquipBtn(true);//토글
                    _inventoryUIView.SetDropBtnInteractable(false);//드랍 비활성
                }
                
                break;
            }
            default:
                break;
        }
    }

    private void HandleOnSetQuickSlot(int quickSlot)
    {
        //퀵슬롯1,2 확인...
        bool isQuick1 = _inventoryManager.ItemQuickSlot1 != null &&
                        _inventoryManager.SelectedItem.Consumable.ItemData ==
                        _inventoryManager.ItemQuickSlot1.ItemData;
        bool isQuick2 = _inventoryManager.ItemQuickSlot2 != null &&
                        _inventoryManager.SelectedItem.Consumable.ItemData ==
                        _inventoryManager.ItemQuickSlot2.ItemData;
        switch (quickSlot)
        {
            case 1://퀵슬롯1 장착버튼
                if (isQuick1)//이미 퀵슬롯1일때
                {
                    //퀵슬롯 1 해제
                    _inventoryManager.SelectedItem.IsEquipped = isQuick2;//퀵슬롯2에 따라 장착여부 (둘 중 하나만 장착해도 장착상태)
                    _inventoryManager.SetQuickSlot1(null);
                    
                    _inventoryUIView.SetDropBtnInteractable(!isQuick2);//퀵슬롯2도 아니면 활성
                    _inventoryUIView.ToggleQuick1Btn(false);//장착 버튼 토글
                    _inventoryUIView.ClearItemQuick1(); //아이콘 비우기
                }
                else
                {
                    //퀵슬롯 1 장착
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.SelectedItem.Consumable;
                    //선택된 아이템 데이터
                    _inventoryManager.SelectedItem.IsEquipped = true;//장착상태
                    _inventoryManager.SetQuickSlot1(item);//퀵슬롯1 장착
                    
                    _inventoryUIView.UpdateItemQuick1(item.ItemData.Icon);//아이콘 업데이트
                    _inventoryUIView.SetDropBtnInteractable(false);//드랍버튼 상호작용 비홯성(장착한 아이템은 드랍불가)
                    _inventoryUIView.ToggleQuick1Btn(true);//장착 버튼 토글
                }
                break;
            case 2://퀵슬롯2 장착버튼
                if (isQuick2)//이미 퀵슬롯2 장착상태일 때
                {
                    _inventoryManager.SelectedItem.IsEquipped = isQuick1;//퀵슬롯1 장착여부에 따라(둘 중 하나만 장착해도 장착상태)
                    _inventoryManager.SetQuickSlot2(null);
                    
                    _inventoryUIView.SetDropBtnInteractable(!isQuick1);//퀵슬롯1도 아니면 상호작용 활성화
                    _inventoryUIView.ToggleQuick2Btn(false);//장착버튼 토글
                    _inventoryUIView.ClearItemQuick2();//아이콘 비우기
                }
                else
                {
                    InventoryManager.ConsumableDataWithQuantity item = _inventoryManager.SelectedItem.Consumable;
                    //현재 선택한 아이템 데이터
                    _inventoryManager.SelectedItem.IsEquipped = true; //장착 상태
                    _inventoryManager.SetQuickSlot2(item);//퀵슬롯2에 장착
                    
                    _inventoryUIView.UpdateItemQuick2(item.ItemData.Icon);//아이콘 업데이트
                    _inventoryUIView.SetDropBtnInteractable(false);//드랍버튼 상호작용 비활성(장착한 아이템은 드랍불가)
                    _inventoryUIView.ToggleQuick2Btn(true);//장착 버튼 토글
                }
                break;
            default:
                break;
        }
    }
    
    private void HandleOnDropButton()//드랍버튼
    {
        ItemTypes itemType = _inventoryManager.SelectedItem.ItemType;
        switch (itemType)
        {
            case ItemTypes.Weapon:
                InventoryManager.WeaponDataWithID weapon = _inventoryManager.SelectedItem.Weapon;
                _inventoryManager.RemoveWeaponData(weapon);//리스트에서 제거
                GameObject dropWeapon = _inventoryManager.ItemPrefabData.GetWeaponPrefab(weapon.ItemData);//GameObject가져오기
                _playerManager.DropItem(dropWeapon);//아이템 드랍 
                HandleOnInventoryOpen(ItemTypes.Weapon);//새로고침
                _inventoryUIView.ClearItemInfo();//아이템정보 clear
                break;
            case ItemTypes.Equipment:
                InventoryManager.EquipmentDataWithID equipment = _inventoryManager.SelectedItem.Equipment;
                _inventoryManager.RemoveEquipmentData(equipment);
                GameObject dropEquipment = _inventoryManager.ItemPrefabData.GetEquipmentPrefab(equipment.ItemData);
                _playerManager.DropItem(dropEquipment);
                HandleOnInventoryOpen(ItemTypes.Equipment);
                _inventoryUIView.ClearItemInfo();
                break;
            case ItemTypes.Consumable:
                InventoryManager.ConsumableDataWithQuantity consumable = _inventoryManager.SelectedItem.Consumable;
                var quantity = consumable.Quantity;
                _inventoryManager.RemoveConsumableData(consumable);
                GameObject dropConsumable = _inventoryManager.ItemPrefabData.GetConsumablePrefab(consumable.ItemData);
                _playerManager.DropItem(dropConsumable);
                HandleOnInventoryOpen(ItemTypes.Consumable);
                if (quantity <= 1)//수량이 1이하였으면
                {
                    _inventoryUIView.ClearItemInfo();
                    _inventoryManager.SelectedItem.Consumable.ItemData = null;
                    _inventoryManager.SelectedItem.Consumable.Quantity = 0;
                }
                else
                {
                    _inventoryUIView.SelectedConsumable(consumable);//아이템 정보 갱신
                }
                break;
            default:
                break;
        }
    }

    private void HandleOnUseItemQuickSlot(int quickSlot)
    {
        switch (quickSlot)
        {
            case 1:
                InventoryManager.ConsumableDataWithQuantity quick1 = _inventoryManager.ItemQuickSlot1;//현재 퀵슬롯1 정보
                if(!quick1.ItemData) return; //null(빈상태)이면 작동x
                _playerManager.UseConsumableItem(_inventoryManager.ItemQuickSlot1.ItemData);//아이템 사용
                if (quick1.Quantity <= 1)
                {
                    _inventoryManager.ItemQuickSlot1.ItemData = null;//수량이 1이하면 null로 초기화
                    _inventoryManager.ItemQuickSlot1.Quantity = 0;
                    _inventoryUIView.ClearItemQuick1();//퀵슬롯1 UI 초기화
                }
                _inventoryManager.RemoveConsumableData(quick1);//인벤토리에서 처리(수량에 따라)
                break;
            case 2:
                InventoryManager.ConsumableDataWithQuantity quick2 = _inventoryManager.ItemQuickSlot2;
                if (!quick2.ItemData) return;
                _playerManager.UseConsumableItem(_inventoryManager.ItemQuickSlot2.ItemData);
                if (quick2.Quantity <= 1)
                {
                    _inventoryManager.ItemQuickSlot2.ItemData = null;
                    _inventoryManager.ItemQuickSlot2.Quantity = 0;
                    _inventoryUIView.ClearItemQuick2();
                }
                _inventoryManager.RemoveConsumableData(quick2);
                break;
        }
    }
    
    public void Dispose()
    {
        _inventoryUIView.OnInventoryOpen -= HandleOnInventoryOpen;
        _inventoryUIView.OnSelectInventorySlot -= HandleSelectInventorySlot;
        _inventoryUIView.OnCurrentWeapon -= HandleShowCurrentWeapon;
        _inventoryUIView.OnCurrentEquipment -= HandleShowCurrentEquipment;
        _inventoryUIView.OnQuickSlot1 -= HandleShowCurrentQuick1;
        _inventoryUIView.OnQuickSlot2 -= HandleShowCurrentQuick2;
      
        _inventoryUIView.OnEquipButton -= HandleOnEquipButton;
        _inventoryUIView.OnDropButton -= HandleOnDropButton;
        _inventoryUIView.OnSetQuickSlot -= HandleOnSetQuickSlot;
        
        _playerManager.OnGetMoney -= HandleAddMoney;
        _playerManager.OnGetItem -= HandleAddItem;
        _playerManager.OnUseItemQuickSlot -= HandleOnUseItemQuickSlot;
        _inventoryManager.OnUpdateMoneyAmount -= HandleUpdateTotalMoney;

    }
}
