using System;
using System.Collections.Generic;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
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
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemRarityText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private Button itemEquipButton;
        [SerializeField] private Button itemDropButton;
        public event Action<ItemTypes> OnInventoryOpen;
        public event Action OnShowIcon;
        public event Action OnCurrentWeapon;
        public event Action OnCurrentEquipment;
        public event Action OnQuickSlot1;
        public event Action OnQuickSlot2;

        private static string RarityToString(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => "Common",
                Rarity.Uncommon => "Uncommon",
                Rarity.Rare => "Rare",
                Rarity.Epic => "Epic",
                Rarity.Legendary => "Legendary",
                _ => rarity.ToString()
            };
        }
        
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

        private void OpenInventory(ItemTypes type)
        {
            inventory.SetActive(true);
            OnInventoryOpen?.Invoke(type);
        }

        public void UpdateCurrentWeapon(Sprite sprite)
        {
            currentWeaponImg.sprite = sprite;
            Color alpha = new Color(1,1,1, 1);
            currentWeaponImg.color = alpha;
        }

        public void UpdateSelectItemIcon(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            Color alpha = new Color(image.color.r, image.color.g, image.color.b, 1);
            image.color = alpha;
        }
        
        public void UpdateInventoryCount(int count, int maxCount)
        {
            countText.text = $"{count} / {maxCount}";
        }
        public void UpdateInventoryIcon(int index, Sprite sprite)
        { 
            Image icon = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
            Color alpha = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            icon.color = alpha;
            icon.sprite = sprite;
        }
        
        public void UpdateInventoryIconWithQuantity(int index, Sprite sprite, int quantity)
        {
            Image icon = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
            Color alpha = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            icon.color = alpha;
            icon.sprite = sprite;
            TextMeshProUGUI quantityText = inventoryGridParent.GetChild(index)
                .transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
            quantityText.text = $"{quantity}";
            quantityText.color = alpha;
        }

        public void ClearInventoryIcon(int maxCount)
        {
            for (int i = 0; i < inventoryGridParent.childCount; i++)
            {
                if (maxCount <= i)
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
                        .transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
                    quantityText.color = alpha;
                }
            }
        }

        public void InitInventory()
        {
            Instantiate(inventoryIconPrefab, inventoryGridParent);
            //인벤토리 슬롯 초기 생성 
        }
        
        public void SelectedWeapon(PlayerWeaponData data)
        {
            UpdateSelectItemIcon(itemImage, data.Icon);
            itemNameText.text = data.WeaponName;
            itemRarityText.text = RarityToString(data.Rarity);
            itemDescriptionText.text = data.Description;
        }

        public void SelectedEquipment(PlayerEquipmentData data)
        {
            UpdateSelectItemIcon(itemImage, data.Icon);
            itemNameText.text = data.EquipmentName;
            itemRarityText.text = RarityToString(data.Rarity);
            itemDescriptionText.text = data.Description;
        }

        public void SelectedConsumable(ConsumableItemData data)
        {
            UpdateSelectItemIcon(itemImage, data.Icon);
            itemNameText.text = data.ItemName;
            itemRarityText.text = RarityToString(data.Rarity);
            itemDescriptionText.text = data.Description;
        }
        
        private void ShowSelectedItemInventory(int index)
        {
            
        }
        
        private void Awake()
        {
            playerButton.onClick.AddListener(OnPlayerStatusMenu);
            weaponButton.onClick.AddListener(() => OpenInventory(ItemTypes.Weapon));
            equipmentButton.onClick.AddListener(() => OpenInventory(ItemTypes.Equipment));
            consumableButton.onClick.AddListener(() => OpenInventory(ItemTypes.Consumable));
            _currentWeaponButton = currentWeaponImg.gameObject.GetComponent<Button>();
            _currentWeaponButton.onClick.AddListener(()=> OnCurrentWeapon?.Invoke());
            _currentEquipmentButton = currentEquipmentImg.gameObject.GetComponent<Button>();
            _currentEquipmentButton.onClick.AddListener(()=>OnCurrentEquipment?.Invoke());
            _currentQuick1Button = currentQuick1Img.gameObject.GetComponent<Button>();
            _currentQuick1Button.onClick.AddListener(()=>OnQuickSlot1?.Invoke());
            _currentQuick2Button = currentQuick2Img.gameObject.GetComponent<Button>();
            _currentQuick2Button.onClick.AddListener(()=>OnQuickSlot2?.Invoke());
        }

        // Update is called once per frame
        void Update()
        {
            //PlayerMenu
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            {
                TogglePlayerMenu();
            }
        }
    }
}
