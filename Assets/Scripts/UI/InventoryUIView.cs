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
        [SerializeField] private Image currentWeaponImg;
        private Button _currentWeaponButton;
        [SerializeField] private Image currentEquipmentImg;
        private Button _currentEquipmentButton;
        [SerializeField] private Image currentQuick1Img;
        private Button _currentQuick1Button;
        [SerializeField] private Image currentQuick2Img;
        private Button _currentQuick2Button;
        
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
        [SerializeField] private Button itemEquipButton;
        [SerializeField] private Image equipButtonInactiveImage;
        [SerializeField] private Button itemDropButton;
        [SerializeField] private Image dropButtonInactiveImage;
        [SerializeField] private TextMeshProUGUI itemValueText;
        [SerializeField] private TextMeshProUGUI itemEffectText;
        
        private ItemTypes _currentInventoryType;
        public event Action<ItemTypes> OnInventoryOpen;
        public event Action<ItemTypes,int> OnSelectInventorySlot;
        public event Action OnCurrentWeapon;
        public event Action OnCurrentEquipment;
        public event Action OnQuickSlot1;
        public event Action OnQuickSlot2;
        
        
        
        private void TogglePlayerMenu()
        {
            playerMenu.SetActive(!playerMenu.activeSelf);
            inventory.SetActive(false);
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

        private void OpenInventory(ItemTypes type)//인벤토리 열기(타입별로)
        {
            inventory.SetActive(true);
            _currentInventoryType = type;
            OnInventoryOpen?.Invoke(type);
        }

        public void UpdateCurrentWeapon(Sprite sprite)
        {
            UpdateSelectItemIcon(currentWeaponImg,sprite);
        }

        public void UpdateCurrentEquipment(Sprite sprite)
        {
            UpdateSelectItemIcon(currentEquipmentImg,sprite);
        }

        public void UpdateCurrentQuick1(Sprite sprite)
        {
            UpdateSelectItemIcon(currentQuick1Img,sprite);
        }

        public void UpdateCurrentQuick2(Sprite sprite)
        {
            UpdateSelectItemIcon(currentQuick2Img,sprite);
        }

        private static void UpdateSelectItemIcon(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            Color alpha = new Color(image.color.r, image.color.g, image.color.b, 1);
            image.color = alpha;//알파값1로 변경. 이미지 sprite로 변경
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
            Color alpha = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            quantityText.text = $"{quantity}";
            quantityText.color = alpha; //수량값도 받아와서 표시.
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
                    Color alpha = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
                    icon.color = alpha;
                    TextMeshProUGUI quantityText = inventoryGridParent.GetChild(i)
                        .transform.Find("SlotBackground/Quantity").GetComponent<TextMeshProUGUI>();
                    quantityText.color = alpha; //알파값 0으로 변경시킴(이전에 있던 아이콘 투명하게)
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
        
        public void SelectedWeapon(PlayerWeaponData data)//ItemInfo에 아이템 데이터 표시
        {
            infoText.SetActive(true);
            UpdateSelectItemIcon(itemImage, data.Icon);
            itemNameText.text = data.WeaponName;
            itemRarityText.text = EnumManager.RarityToString(data.Rarity);
            itemTypeText.text = $"{EnumManager.WeaponTypeToString(data.WeaponType)}" +
                                $" / {EnumManager.AttackTypeToString(data.AttackType)}";
            itemValueText.text = $"공격력: {data.AttackValue}";
            itemEffectText.text = data.ItemEffects.Length != 0 ? "Have Effect" : "None."; //Temp. 효과별로 항복마다 처리 필요
            itemDescriptionText.text = data.Description;
        }

        public void SelectedEquipment(PlayerEquipmentData data)
        {
            infoText.SetActive(true);
            UpdateSelectItemIcon(itemImage, data.Icon);
            itemNameText.text = data.EquipmentName;
            itemRarityText.text = EnumManager.RarityToString(data.Rarity);
            itemTypeText.text = data.Type;
            itemValueText.text = $"방어력: {data.DefenseValue}";
            itemEffectText.text = data.ItemEffect.Length != 0 ? "Have Effect" : "None."; //Temp. 효과별로 항복마다 처리 필요
            itemDescriptionText.text = data.Description;
        }
        public void SelectedConsumable(InventoryManager.ConsumableDataWithQuantity consumableItem)
        {
            infoText.SetActive(true);
            ConsumableItemData data = consumableItem.ItemData;
            UpdateSelectItemIcon(itemImage,data.Icon);
            itemNameText.text = data.ItemName;
            itemRarityText.text = EnumManager.RarityToString(data.Rarity);
            itemTypeText.text = EnumManager.ConsumableTypeToString(data.Type);
            itemValueText.text = $"수량: {consumableItem.Quantity}";
            itemEffectText.text = data.ItemData.Length != 0 ? "Have Effect" : "None.";
            itemDescriptionText.text = data.Description;
        }
        
        public void ItemEquipButtonActive(bool isActive)//장착,드랍 버튼 활성/비활성
        {
            itemEquipButton.interactable = isActive;
            equipButtonInactiveImage.gameObject.SetActive(!isActive);
        }
        public void ItemDropButtonActive(bool isActive)//장착,드랍 버튼 활성/비활성
        {
            itemDropButton.interactable = isActive;
            dropButtonInactiveImage.gameObject.SetActive(!isActive);
        }
        private void SelectItemInventory(ItemTypes  itemTypes,int index)
        {
            //Debug.Log($"Selected Item!: {itemTypes.ToString()} {index}");
            OnSelectInventorySlot?.Invoke(itemTypes, index);//현재 클릭한 인벤토리 슬롯 이벤트
        }
        
        private void Awake()
        {
            _currentInventoryType = ItemTypes.Weapon;//default
            playerButton.onClick.AddListener(OnPlayerStatusMenu);
            weaponButton.onClick.AddListener(() => OpenInventory(ItemTypes.Weapon));
            equipmentButton.onClick.AddListener(() => OpenInventory(ItemTypes.Equipment));
            consumableButton.onClick.AddListener(() => OpenInventory(ItemTypes.Consumable));
            _currentWeaponButton = currentWeaponImg.gameObject.GetComponent<Button>();
            _currentWeaponButton.onClick.AddListener(() => OnCurrentWeapon?.Invoke());//현재 장착한 아이템 이벤트
            _currentEquipmentButton = currentEquipmentImg.gameObject.GetComponent<Button>();
            _currentEquipmentButton.onClick.AddListener(() => OnCurrentEquipment?.Invoke());
            _currentQuick1Button = currentQuick1Img.gameObject.GetComponent<Button>();
            _currentQuick1Button.onClick.AddListener(() => OnQuickSlot1?.Invoke());
            _currentQuick2Button = currentQuick2Img.gameObject.GetComponent<Button>();
            _currentQuick2Button.onClick.AddListener(() => OnQuickSlot2?.Invoke());
        }

        private void Update()
        {
            //PlayerMenu
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            {
                TogglePlayerMenu();
            }
        }
    }
}
