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
        _playerManager.OnPlayerDeath += HandlePlayerDeath;
        
        _inventoryUIView.OnEquipButton += HandleOnEquipButton;
        _inventoryUIView.OnDropButton += HandleOnDropButton;
        _inventoryUIView.OnSetQuickSlot += HandleOnSetQuickSlot;


        //init
        //기본무기 설정-인벤토리/UI
        var defaultWeaponData = _playerManager.PlayerDefaultWeaponData;
        var defaultWeaponPrefab = _playerManager.PlayerDefaultWeaponData.GetItemPrefab();
        var defaultWeaponWithID = new ItemDataWithID()
        {
            ItemData = defaultWeaponData,
            ItemPrefab = defaultWeaponPrefab,
            ItemID = 1,
        };
        _inventoryManager.AddItemData(defaultWeaponWithID);
        _inventoryManager.SetWeapon(defaultWeaponWithID);
        _inventoryUIView.UpdateEquippedWeapon(defaultWeaponData.GetIcon());

        int maxCount = _inventoryManager.weaponInventoryMaxCount; //생성은 초기화, 최대칸 증가 시에만
        for (int i = 0; i < maxCount; i++)
        {
            _inventoryUIView.InitInventory();
        }

    }

    private void HandlePlayerDeath()
    {
        //
        _inventoryManager.ResetInventory();
        var defaultWeaponData = _playerManager.PlayerDefaultWeaponData;
        var defaultWeaponPrefab = _playerManager.PlayerDefaultWeaponData.GetItemPrefab();
        var defaultWeaponWithID = new ItemDataWithID()
        {
            ItemData = defaultWeaponData,
            ItemPrefab = defaultWeaponPrefab,
            ItemID = 1,
        };
        _inventoryManager.AddItemData(defaultWeaponWithID);
        _inventoryManager.SetWeapon(defaultWeaponWithID);
        _inventoryUIView.UpdateEquippedWeapon(defaultWeaponData.GetIcon());
        _inventoryUIView.ResetInventoryUI();
    }

private void HandleShowCurrentWeapon() //현재 무기 데이터 표시
    {
        var itemDataWithID = _inventoryManager.CurrentWeaponData;
        if (itemDataWithID.ItemData == null) return;
        _inventoryUIView.SelectedItem(itemDataWithID);
        _inventoryUIView.SetEquipBtnInteractable(false);//무기는 무조건 하나 장착해야함.(장착해제x)
        _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍 불가

        _inventoryManager.SetSelectedItem(itemDataWithID);
    }
    
    private void HandleShowCurrentEquipment()
    {
        var itemDataWithID = _inventoryManager.CurrentEquipmentData;
        if (itemDataWithID.ItemData == null) return;
        _inventoryUIView.SelectedItem(itemDataWithID);
        _inventoryUIView.ToggleItemEquipBtn(true);//장착버튼 토글.
        _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍 불가
            
        _inventoryManager.SetSelectedItem(itemDataWithID);

    }
    private void HandleShowCurrentQuick1()
    {
        var itemDataWithID = _inventoryManager.ItemQuickSlot1Data;
        if (itemDataWithID.ItemData == null) return;
        _inventoryUIView.SelectedItem(itemDataWithID);
        _inventoryUIView.ToggleQuick1Btn(true);
        _inventoryUIView.ToggleQuick2Btn(_inventoryManager.ItemQuickSlot2Data != null 
                                         && itemDataWithID.ItemData == _inventoryManager.ItemQuickSlot2Data.ItemData);
        //다른 퀵슬롯에 장착된 상태인지 확인
        _inventoryUIView.SetDropBtnInteractable(false);//장착한 아이템은 드랍불가
            
        _inventoryManager.SetSelectedItem(itemDataWithID);

    }

    private void HandleShowCurrentQuick2()
    {
        var itemDataWithID = _inventoryManager.ItemQuickSlot2Data;
        if (itemDataWithID.ItemData == null) return;
        _inventoryUIView.SelectedItem(itemDataWithID);
        _inventoryUIView.ToggleQuick1Btn(_inventoryManager.ItemQuickSlot1Data != null 
                                         && itemDataWithID.ItemData == _inventoryManager.ItemQuickSlot1Data.ItemData);
        _inventoryUIView.ToggleQuick2Btn(true);
        _inventoryUIView.SetDropBtnInteractable(false);    
            
        _inventoryManager.SetSelectedItem(itemDataWithID);
    }

    private void HandleSelectInventorySlot(ItemTypes type, int index) //선택한 인벤토리 슬롯 아이템 데이터 표시
    {
        switch (type)//아이템 타입 따라
        {
            case ItemTypes.Weapon:
                if (index < _inventoryManager.weaponInventoryCount)
                {
                    var itemDataWithID = _inventoryManager.WeaponDataList[index];//선택한 아이템 데이터
                    bool isEquipped = itemDataWithID.ItemID == _inventoryManager.CurrentWeaponData.ItemID;//장착한 아이템과 인덱스 비교
                    _inventoryUIView.SelectedItem(itemDataWithID);//데이터 표시
                    if (itemDataWithID.ItemData is PlayerWeaponData weaponData)
                    {
                        if (weaponData.IsDefaultWeapon || isEquipped) //장착한 무기나 기본 무기는 드랍불가
                        {
                            _inventoryUIView.SetDropBtnInteractable(false);
                        }
                        else
                        {
                            _inventoryUIView.SetDropBtnInteractable(true);
                        }
                        _inventoryUIView.ToggleItemEquipBtn(isEquipped);
                        _inventoryUIView.SetEquipBtnInteractable(!isEquipped);//장착한 무기는 장착버튼 비활성
                    
                        _inventoryManager.SetSelectedItem(itemDataWithID);
                    }
                    
                }
                break;
            case ItemTypes.Equipment:
                if (index < _inventoryManager.equipmentInventoryCount)
                {
                    var itemDataWithID = _inventoryManager.EquipmentDataList[index];
                    bool isEquipped;
                    if (_inventoryManager.CurrentEquipmentData == null)
                    {
                        isEquipped = false;
                    }
                    else
                    {
                        isEquipped = itemDataWithID.ItemID == _inventoryManager.CurrentEquipmentData.ItemID; 
                    }
                    _inventoryUIView.SelectedItem(itemDataWithID);//데이터 표시
                    _inventoryUIView.ToggleItemEquipBtn(isEquipped);
                    _inventoryUIView.SetDropBtnInteractable(!isEquipped);
                    
                    _inventoryManager.SetSelectedItem(itemDataWithID);
                }
                break;
            case ItemTypes.Consumable:
                if (index < _inventoryManager.consumableInventoryCount)//소모템은 인덱스없음(중복가능해서 현재는 없음)
                {
                    var itemDataWithID = _inventoryManager.ConsumableDataList[index];
                    _inventoryUIView.SelectedItem(itemDataWithID);
                    //퀵슬롯 설정
                    bool isQuick1 = _inventoryManager.ItemQuickSlot1Data != null &&//null확인(빈 상태인지)
                                    itemDataWithID.ItemData == _inventoryManager.ItemQuickSlot1Data.ItemData;//퀵슬롯 장착 확인
                    bool isQuick2 = _inventoryManager.ItemQuickSlot2Data != null &&
                                    itemDataWithID.ItemData == _inventoryManager.ItemQuickSlot2Data.ItemData;
                    
                    _inventoryUIView.ToggleQuick1Btn(isQuick1);
                    _inventoryUIView.ToggleQuick2Btn(isQuick2);
                    _inventoryUIView.SetDropBtnInteractable(!(isQuick1||isQuick2)); //수량따라 작동하게 수정필요
                    
                    _inventoryManager.SetSelectedItem(itemDataWithID);
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
                    _inventoryUIView.UpdateInventoryIcon(index, weaponData.ItemData.GetIcon());
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
                    _inventoryUIView.UpdateInventoryIcon(index, equipmentData.ItemData.GetIcon());
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
                    int quantity = consumableItem.ItemQuantity;//수량도 표시
                    _inventoryUIView.UpdateInventoryIconWithQuantity(index, consumableItem.ItemData.GetIcon(), quantity);
                    index++;
                }
                break;
        }
    }

    private void HandleAddMoney(int money)//돈 획득
    {
        _inventoryManager.AddMoney(money);
    }

    private void HandleAddItem(ItemDataWithID itemDataWithID)//아이템 획득
    {
        var itemData = itemDataWithID.ItemData;
        var itemIcon = itemData.GetIcon();
        var quick1 = _inventoryManager.ItemQuickSlot1Data;
        var quick2 = _inventoryManager.ItemQuickSlot2Data;
        
        _inventoryManager.AddItemData(itemDataWithID); //아이템 데이터 저장
        
        if (itemData is not ConsumableItemData) return; //소비 아이템이면
        
        if (quick1 == null) //소모템이면 퀵슬롯 빈 상태면 등록(1번 먼저)
        {
            if (quick2 != null && itemData == quick2.ItemData) //퀵슬롯2이 얻은 아이템과 똑같은 아이템이면(IItemData로 비교)
            {
                _inventoryUIView.UpdateItemQuick2Quantity(quick2.ItemQuantity);//퀵슬롯2 UI 업데이트
            }
            else
            {
                _inventoryManager.SetQuickSlot1(itemDataWithID); //퀵슬롯1 매개변수 할당
                _inventoryUIView.UpdateItemQuick1(itemIcon,itemDataWithID.ItemQuantity);//퀵슬롯 UI 업데이트
            }
        }
        else if(quick2 == null)
        {
            if (itemData == quick1.ItemData)
            {
                _inventoryUIView.UpdateItemQuick1Quantity(quick1.ItemQuantity);
            }
            else
            {
                _inventoryManager.SetQuickSlot2(itemDataWithID);
                _inventoryUIView.UpdateItemQuick2(itemIcon, itemDataWithID.ItemQuantity);
            }
        }
        else
        {
            //퀵슬롯에 있는 아이템이라면
            if (itemData == quick1.ItemData)
            {
                _inventoryUIView.UpdateItemQuick1Quantity(quick1.ItemQuantity); //수량만 업데이트
            }
            else if (itemData == quick2.ItemData)
            {
                _inventoryUIView.UpdateItemQuick2Quantity(quick2.ItemQuantity);
            }
        }
    }

    private void HandleUpdateTotalMoney(int moneyAmount)//현재 보유한 돈 표시
    {
        _inventoryUIView.UpdateMoney(moneyAmount);
    }

    private void HandleOnEquipButton()//장착버튼 ->스탯 반영
    {
        ItemDataWithID itemDataWithID = _inventoryManager.SelectedItem;
        switch (itemDataWithID.ItemData)//아이템 타입 따라
        {
            case PlayerWeaponData weaponData: //항상 장착만(장착 해제 불가, 무기 하나는 들고있어야함)
            {
                
                _inventoryManager.SetWeapon(itemDataWithID);
                //Debug.Log(itemDataWithID.ItemPrefab);
                _playerManager.PlayerEquipWeapon(weaponData, itemDataWithID.ItemPrefab);//player equip weapon
                
                _inventoryUIView.UpdateEquippedWeapon(weaponData.GetIcon());//현재 장착 무기 아이콘
                _inventoryUIView.SetEquipBtnInteractable(false);
                _inventoryUIView.SetDropBtnInteractable(false);//장착, 드랍 상호작용 비활성
                break;
            }
            case PlayerEquipmentData equipmentData:
            {
                var currentEquipment = _inventoryManager.CurrentEquipmentData;
                if (currentEquipment != null && _inventoryManager.SelectedItem.ItemID 
                    == currentEquipment.ItemID)//장착된 아이템인지 확인
                {
                    _inventoryManager.SetEquipment(null); //장착 장비 초기화
                    _playerManager.PlayerEquipEquipment(null, null);
                    
                    _inventoryUIView.ToggleItemEquipBtn(false); //장착버튼 토글(장착/해제 텍스트)
                    _inventoryUIView.SetDropBtnInteractable(true); //드랍 가능
                    _inventoryUIView.ClearEquippedEquipment(); //PlayerStatus에서 비활성
                }
                else
                {
                    _inventoryManager.SetEquipment(itemDataWithID);
                    _playerManager.PlayerEquipEquipment(equipmentData, itemDataWithID.ItemPrefab);
                    
                    _inventoryUIView.UpdateEquippedEquipment(equipmentData.GetIcon());//아이콘 업데이트
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
        bool isQuick1 = _inventoryManager.ItemQuickSlot1Data != null &&
                        _inventoryManager.SelectedItem ==
                        _inventoryManager.ItemQuickSlot1Data;
        bool isQuick2 = _inventoryManager.ItemQuickSlot2Data != null &&
                        _inventoryManager.SelectedItem ==
                        _inventoryManager.ItemQuickSlot2Data;
        switch (quickSlot)
        {
            case 1://퀵슬롯1 장착버튼
                if (isQuick1)//이미 퀵슬롯1일때
                {
                    //퀵슬롯 1 해제
                    _inventoryManager.SetQuickSlot1(null);
                    
                    _inventoryUIView.SetDropBtnInteractable(!isQuick2);//퀵슬롯2도 아니면 활성
                    _inventoryUIView.ToggleQuick1Btn(false);//장착 버튼 토글
                    _inventoryUIView.ClearItemQuick1(); //아이콘 비우기
                }
                else
                {
                    //퀵슬롯 1 장착
                    ItemDataWithID item = _inventoryManager.SelectedItem;
                    //선택된 아이템 데이터
                    _inventoryManager.SetQuickSlot1(item);//퀵슬롯1 장착
                   
                    _inventoryUIView.UpdateItemQuick1(item.ItemData.GetIcon(),item.ItemQuantity);//아이콘 업데이트
                    _inventoryUIView.SetDropBtnInteractable(false);//드랍버튼 상호작용 비홯성(장착한 아이템은 드랍불가)
                    _inventoryUIView.ToggleQuick1Btn(true);//장착 버튼 토글
                }
                break;
            case 2://퀵슬롯2 장착버튼
                if (isQuick2)//이미 퀵슬롯2 장착상태일 때
                {
                    _inventoryManager.SetQuickSlot2(null);
                    
                    _inventoryUIView.SetDropBtnInteractable(!isQuick1);//퀵슬롯1도 아니면 상호작용 활성화
                    _inventoryUIView.ToggleQuick2Btn(false);//장착버튼 토글
                    _inventoryUIView.ClearItemQuick2();//아이콘 비우기
                }
                else
                {
                    ItemDataWithID item = _inventoryManager.SelectedItem;
                    //현재 선택한 아이템 데이터
                    _inventoryManager.SetQuickSlot2(item);//퀵슬롯2에 장착
                    
                    _inventoryUIView.UpdateItemQuick2(item.ItemData.GetIcon(),item.ItemQuantity);//아이콘 업데이트
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
        //ItemTypes itemType = _inventoryManager.SelectedItem.ItemType;
        var itemDataWithID = _inventoryManager.SelectedItem;
        
        _inventoryManager.RemoveItemData(itemDataWithID);//리스트에서 제거
        var dropItem = itemDataWithID.ItemPrefab;
        Debug.Log(dropItem.name);
        _playerManager.DropItem(dropItem);
        switch (itemDataWithID.ItemData)
        {
            case PlayerWeaponData weaponData:
                HandleOnInventoryOpen(ItemTypes.Weapon);//새로고침
                _inventoryUIView.ClearItemInfo();//아이템정보 clear
                break;
            case PlayerEquipmentData equipmentData:
                HandleOnInventoryOpen(ItemTypes.Equipment);
                _inventoryUIView.ClearItemInfo();
                break;
            case ConsumableItemData consumableItemData:
                var quantity = itemDataWithID.ItemQuantity;
                HandleOnInventoryOpen(ItemTypes.Consumable);
                if (quantity <= 1)//수량이 1이하였으면
                {
                    _inventoryUIView.ClearItemInfo();
                    _inventoryManager.SetSelectedItem(null);
                }
                else
                {
                    _inventoryUIView.SelectedItem(itemDataWithID);//아이템 정보 갱신
                }
                break;
            default:
                break;
        }
    }

    private void HandleOnUseItemQuickSlot(int quickSlot)
    {
        var quick1 = _inventoryManager.ItemQuickSlot1Data;//현재 퀵슬롯1 정보
        var quick2 = _inventoryManager.ItemQuickSlot2Data;
        switch (quickSlot)
        {
            
            case 1:
                if(quick1 == null) return; //null(빈상태)이면 작동x
                if (quick1.ItemData is ConsumableItemData consumableItemData1)
                {
                    int quantity = quick1.ItemQuantity;
                    _playerManager.UseConsumableItem(consumableItemData1);//아이템 사용
                    _inventoryManager.RemoveItemData(quick1);//인벤토리에서 처리(수량에 따라)
                    _inventoryUIView.UpdateItemQuick1Quantity(quantity-1);
                    if (quantity <= 1)
                    {
                        _inventoryManager.SetQuickSlot1(null);
                        _inventoryUIView.ClearItemQuick1();//퀵슬롯1 UI 초기화
                    }

                    if (quick1 == quick2) //퀵슬롯 1,2가 같으면 퀵슬롯2도
                    {
                        _inventoryUIView.UpdateItemQuick2Quantity(quantity-1);
                        if (quantity <= 1)
                        {
                            _inventoryManager.SetQuickSlot2(null);
                            _inventoryUIView.ClearItemQuick2();
                        }
                    }
                }
                break;
            case 2:
                if (quick2 == null) return;
                if (quick2.ItemData is ConsumableItemData consumableItemData2)
                {
                    int quantity = quick2.ItemQuantity;
                    _playerManager.UseConsumableItem(consumableItemData2);
                    _inventoryManager.RemoveItemData(quick2);
                    _inventoryUIView.UpdateItemQuick2Quantity(quantity-1);
                    if (quantity <= 1)
                    {
                        _inventoryManager.SetQuickSlot2(null);
                        _inventoryUIView.ClearItemQuick2();
                    }

                    if (quick2 == quick1)
                    {
                        _inventoryUIView.UpdateItemQuick1Quantity(quantity-1);
                        if (quantity <= 1)
                        {
                            _inventoryManager.SetQuickSlot1(null);
                            _inventoryUIView.ClearItemQuick1();
                        }
                    }
                }
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
