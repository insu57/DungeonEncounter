using System;
using System.Collections.Generic;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InventoryUIView : MonoBehaviour
    {
        //PlayerMenu...Inventory
        [Header("Player Menu")]
        [SerializeField] private GameObject playerMenu;
        [SerializeField] private TextMeshProUGUI jobText;
        [SerializeField] private TextMeshProUGUI moneyMainText;//MainUI
        [SerializeField] private TextMeshProUGUI moneyMenuText;
        [SerializeField] private Image equippedWeaponImg;
        private Button _equippedWeaponButton;
        [SerializeField] private Image equippedEquipmentImg;
        private Button _equippedEquipmentButton;
        [SerializeField] private Image itemQuick1Img;
        [SerializeField] private TextMeshProUGUI itemQuick1Quantity;
        [SerializeField] private Image itemQuick1PlayerInfoImage; //PlayerUI
        [SerializeField] private TextMeshProUGUI itemQuick1PlayerInfoQuantity;
        private Button _itemQuick1Button;
        [SerializeField] private Image itemQuick2Img;
        [SerializeField] private TextMeshProUGUI itemQuick2Quantity;
        [SerializeField] private Image itemQuick2PlayerInfoImage;
        [SerializeField] private TextMeshProUGUI itemQuick2PlayerInfoQuantity;
        private Button _itemQuick2Button;
        
        [Header("Inventory")]
        [SerializeField] private GameObject inventory;
        [SerializeField] private Transform inventoryGridParent;
        [SerializeField] private GameObject inventoryIconPrefab;
        [SerializeField] private Button playerButton;
        [SerializeField] private Button weaponButton;
        [SerializeField] private Button equipmentButton;
        [SerializeField] private Button consumableButton;
        [SerializeField] private TextMeshProUGUI countText;
        
        [Header("Item Info")]
        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject infoText;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemRarityText;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI itemValueText;
        [SerializeField] private TextMeshProUGUI itemEffectText;
        [Header("Item Info Button")]
        [SerializeField] private Button itemEquipButton;
        [SerializeField] private TextMeshProUGUI itemEquipButtonText;
        [SerializeField] private Image equipButtonInactiveImage;
        [SerializeField] private GameObject setQuickSlot;
        [SerializeField] private Button setQuick1Button;
        [SerializeField] private TextMeshProUGUI setQuick1ButtonText;
        //[SerializeField] private Image quick1InactiveImage;
        [SerializeField] private Button setQuick2Button;
        //[SerializeField] private Image quick2InactiveImage;
        [SerializeField] private TextMeshProUGUI setQuick2ButtonText;
        [SerializeField] private Button itemDropButton;
        [SerializeField] private Image dropButtonInactiveImage;
        
        private ItemTypes _currentInventoryType;
        public event Action<ItemTypes> OnInventoryOpen;
        public event Action<ItemTypes,int> OnSelectInventorySlot;
        public event Action OnCurrentWeapon;
        public event Action OnCurrentEquipment;
        public event Action OnQuickSlot1;
        public event Action OnQuickSlot2;
        public event Action OnEquipButton;
        public event Action OnDropButton;
        public event Action<int> OnSetQuickSlot;

        public void ResetInventoryUI()
        {
            ClearEquippedEquipment();
            ClearItemQuick1();
            ClearItemQuick2();
            playerMenu.SetActive(false);
            ClearItemInfo();
        }
        
        public void TogglePlayerMenu()
        {
            playerMenu.SetActive(!playerMenu.activeSelf);
            inventory.SetActive(false);
            ClearItemInfo();
        }
        
        public void UpdateMoney(int money)
        {
            moneyMainText.text = money.ToString();
            moneyMenuText.text = money.ToString();
        }

        private void OnPlayerStatusMenu()
        {
            inventory.SetActive(false);
        }

        private void OpenInventory(ItemTypes type)//인벤토리 열기 이벤트(타입별로)
        {
            inventory.SetActive(true);
            _currentInventoryType = type;
            OnInventoryOpen?.Invoke(type);
        }

        public void UpdateEquippedWeapon(Sprite sprite)
        {
            UpdateSelectItemIcon(equippedWeaponImg,sprite);
        }
        
        public void UpdateEquippedEquipment(Sprite sprite)
        {
            UpdateSelectItemIcon(equippedEquipmentImg,sprite);
        }
        public void ClearEquippedEquipment()
        {
            equippedEquipmentImg.gameObject.SetActive(false);
        }  
        public void UpdateItemQuick1(Sprite sprite, int quantity)
        {
            UpdateSelectItemIcon(itemQuick1Img,sprite);
            UpdateSelectItemIcon(itemQuick1PlayerInfoImage,sprite);
            UpdateItemQuick1Quantity(quantity);
        }

        public void UpdateItemQuick1Quantity(int quantity)
        {
            itemQuick1Quantity.text = quantity.ToString();
            itemQuick1PlayerInfoQuantity.text = quantity.ToString();
        }

        public void ClearItemQuick1()
        {
            itemQuick1Img.gameObject.SetActive(false);
            itemQuick1PlayerInfoImage.gameObject.SetActive(false);
        }
        public void UpdateItemQuick2(Sprite sprite, int quantity)
        {
            UpdateSelectItemIcon(itemQuick2Img,sprite);
            UpdateSelectItemIcon(itemQuick2PlayerInfoImage,sprite);
            UpdateItemQuick2Quantity(quantity);
        }

        public void UpdateItemQuick2Quantity(int quantity)
        {
            itemQuick2Quantity.text = quantity.ToString();
            itemQuick2PlayerInfoQuantity.text = quantity.ToString();
        }

        public void ClearItemQuick2()
        {
            itemQuick2Img.gameObject.SetActive(false);
            itemQuick2PlayerInfoImage.gameObject.SetActive(false);
        }
        private static void UpdateSelectItemIcon(Image image, Sprite sprite)
        {
            image.gameObject.SetActive(true);
            image.sprite = sprite;
        }
        
        public void UpdateInventoryCount(int count, int maxCount)
        {
            countText.text = $"{count} / {maxCount}"; //현재개수 / 최대개수
        }
        public void UpdateInventoryIcon(int index, Sprite sprite)
        { 
            Image icon = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
            UpdateSelectItemIcon(icon, sprite);
        }
        
        public void UpdateInventoryIconWithQuantity(int index, Sprite sprite, int quantity)
        {
            Image icon = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
            UpdateSelectItemIcon(icon, sprite);
            TextMeshProUGUI quantityText = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/Quantity").GetComponent<TextMeshProUGUI>();
            quantityText.gameObject.SetActive(true);
            quantityText.text = $"{quantity}"; //수량값도 받아와서 표시.
        }

        public void ClearInventoryIcon(int maxCount)//인벤토리 초기화.
        {
            for (int i = 0; i < inventoryGridParent.childCount; i++)
            {
                if (maxCount <= i)//인벤토리 최대개수 넘는 슬롯 비활성
                {
                    inventoryGridParent.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    inventoryGridParent.GetChild(i).gameObject.SetActive(true);
                    Image icon = inventoryGridParent.GetChild(i)
                        .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
                    icon.gameObject.SetActive(false);
                    TextMeshProUGUI quantityText = inventoryGridParent.GetChild(i)
                        .transform.Find("SlotBackground/Quantity").GetComponent<TextMeshProUGUI>();
                    quantityText.gameObject.SetActive(false);
                    //아이템 이미지, 수량 텍스트 비활성.
                }
            }
        }

        public void InitInventory()
        {
            int count  = inventoryGridParent.childCount;
            GameObject inventorySlot = Instantiate(inventoryIconPrefab, inventoryGridParent);
            Button slotButton = inventorySlot.GetComponentInChildren<Button>();
            slotButton.onClick.AddListener(
                ()=> SelectItemInventory(_currentInventoryType, count));//콜백이벤트 등록
            //인벤토리 슬롯 초기 생성 
        }

        public void SelectedItem(ItemDataWithID itemDataWithID)
        {
            IItemData itemData = itemDataWithID.ItemData;
            infoText.SetActive(true);
            UpdateSelectItemIcon(itemImage, itemData.GetIcon());
            itemNameText.text = itemData.GetName();
            itemDescriptionText.text = itemData.GetDescription();
            itemRarityText.text = EnumManager.RarityToString(itemData.GetRarity());

            if (itemData.GetEffects().Length != 0)
            {
                foreach (var itemEffect in itemData.GetEffects())
                {
                    itemEffectText.text = $"{itemEffect.effectDescription}\n";
                }
            }
            else
            {
                itemEffectText.text = "None.";
            }
            
            switch (itemData)
            {
                case PlayerWeaponData weaponData:
                    SetEquipBtnInteractable(true);
                    SetQuickSlotBtnActive(false);
                    itemTypeText.text = $"{EnumManager.WeaponTypeToString(weaponData.WeaponType)}" +
                                        $" / {EnumManager.AttackTypeToString(weaponData.AttackType)}";
                    itemValueText.text = $"공격력: {weaponData.AttackValue}";
                    break;
                case PlayerEquipmentData equipmentData:
                    SetEquipBtnInteractable(true);
                    SetQuickSlotBtnActive(false);
                    itemTypeText.text = equipmentData.Type;
                    itemValueText.text = $"방어력: {equipmentData.DefenseValue}";
                    break;
                case ConsumableItemData consumableData:
                    SetQuickSlotBtnActive(true);
                    itemTypeText.text = EnumManager.ConsumableTypeToString(consumableData.Type);
                    itemValueText.text = $"수량: {itemDataWithID.ItemQuantity}";
                    break;
            }
        }
        
        public void SetEquipBtnInteractable(bool isActive)//장착버튼 상호작용 활성/비활성
        {
            itemEquipButton.interactable = isActive;
            equipButtonInactiveImage.gameObject.SetActive(!isActive);
        }
        public void SetDropBtnInteractable(bool isActive)//드랍버튼 상호작용  활성/비활성
        {
            itemDropButton.interactable = isActive;
            dropButtonInactiveImage.gameObject.SetActive(!isActive);
        }

        public void SetQuickSlotBtnActive(bool isActive)//퀵슬롯 장착 버튼
        {
            setQuickSlot.SetActive(isActive);
            itemEquipButton.gameObject.SetActive(!isActive);
        }

        public void ToggleItemEquipBtn(bool isEquipped) //장착 토글
        {
            itemEquipButtonText.text = isEquipped ? "장착해제" : "장착하기";
        }

        public void ToggleQuick1Btn(bool isEquipped) //장착 토글...퀵슬롯
        {
            setQuick1ButtonText.text = isEquipped ? "해제" : "장착";
        }

        public void ToggleQuick2Btn(bool isEquipped)
        {
            setQuick2ButtonText.text = isEquipped ? "해제" : "장착";
        }

        public void ClearItemInfo()//초기화
        {
            infoText.SetActive(false);
            itemImage.gameObject.SetActive(false);
            SetQuickSlotBtnActive(false);
            ToggleItemEquipBtn(false);
            ToggleQuick1Btn(false);
            ToggleQuick2Btn(false);
            SetEquipBtnInteractable(false);
            SetDropBtnInteractable(false);
        }
        
        private void SelectItemInventory(ItemTypes  itemTypes,int index)
        {
            OnSelectInventorySlot?.Invoke(itemTypes, index);//현재 클릭한 인벤토리 슬롯 이벤트
        }
        
        private void Awake()
        {
            _currentInventoryType = ItemTypes.Weapon;//default
            playerButton.onClick.AddListener(OnPlayerStatusMenu);
            weaponButton.onClick.AddListener(() => OpenInventory(ItemTypes.Weapon));
            equipmentButton.onClick.AddListener(() => OpenInventory(ItemTypes.Equipment));
            consumableButton.onClick.AddListener(() => OpenInventory(ItemTypes.Consumable));
           
            _equippedWeaponButton = equippedWeaponImg.gameObject.GetComponent<Button>();
            _equippedWeaponButton.onClick.AddListener(() => OnCurrentWeapon?.Invoke());//현재 장착한 아이템 이벤트
            _equippedEquipmentButton = equippedEquipmentImg.gameObject.GetComponent<Button>();
            _equippedEquipmentButton.onClick.AddListener(() => OnCurrentEquipment?.Invoke());
            _itemQuick1Button = itemQuick1Img.gameObject.GetComponent<Button>();
            _itemQuick1Button.onClick.AddListener(() => OnQuickSlot1?.Invoke());
            _itemQuick2Button = itemQuick2Img.gameObject.GetComponent<Button>();
            _itemQuick2Button.onClick.AddListener(() => OnQuickSlot2?.Invoke());
            
            itemEquipButton.onClick.AddListener(()=>OnEquipButton?.Invoke());
            itemDropButton.onClick.AddListener(()=>OnDropButton?.Invoke());
            setQuick1Button.onClick.AddListener(() => OnSetQuickSlot?.Invoke(1));
            setQuick2Button.onClick.AddListener(() => OnSetQuickSlot?.Invoke(2));
        }
    }
}
