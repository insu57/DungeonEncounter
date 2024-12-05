using System;
using System.Collections.Generic;
using Scriptable_Objects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIView : MonoBehaviour
    {
        [SerializeField] private Canvas canvasMain;
        //Game Menu
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Button resumeButton;
    
        //Player Info
        [Header("Player Info")]
        //직업 관련 추가 필요
        [SerializeField] private Image playerHealthBar;
        [SerializeField] private Image playerEnergyBar;
        
    
        //PlayerMenu...Inventory
        [Header("Player Menu")]
        [SerializeField] private GameObject playerMenu;
        [SerializeField] private TextMeshProUGUI jobText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private Image currentWeaponImg;
        [SerializeField] private Image currentEquipmentImg;
        [SerializeField] private Image currentQuick1Img;
        [SerializeField] private Image currentQuick2Img;
        
        [Header("Inventory")]
        [SerializeField] private GameObject inventory;
        [SerializeField] private Transform inventoryGridParent;
        
        [SerializeField] private GameObject inventoryIconPrefab;
        [SerializeField] private Button playerButton;
        [SerializeField] private Button weaponButton;
        [SerializeField] private Button equipmentButton;
        [SerializeField] private Button consumableButton;
        [SerializeField] private TextMeshProUGUI countText;
        
        public event Action<ItemTypes> OnInventoryOpen;
        public event Action OnWeaponInventory;
        public event Action OnEquipmentInventory;
        public event Action OnConsumableInventory;
        public event Action OnShowIcon; //icon? data?
        
        
        private void TogglePause()
        {
            GameManager.Instance.TogglePause();
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }

        private void TogglePlayerMenu()
        {
            playerMenu.SetActive(!playerMenu.activeSelf);
            inventory.SetActive(false);
        }
        
        public void UpdatePlayerHealthBar(float health, float maxHealth)
        {
            playerHealthBar.fillAmount = health / maxHealth;
            healthText.text = $"{health}/{maxHealth}";
            Debug.Log($"{health}/{maxHealth}");
        }

        public void UpdatePlayerEnergyBar(float energy, float maxEnergy)
        {
            playerEnergyBar.fillAmount = energy / maxEnergy;
            energyText.text = $"{energy}/{maxEnergy}";
        }
        //플레이어 정보 창일때만 호출하게 수정필요
        public void UpdatePlayerAttackValue(float attackValue)
        {
            attackText.text = $"{attackValue}";
        }

        public void UpdatePlayerDefenseValue(float defenseValue)
        {
            defenseText.text = $"{defenseValue}";
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

        public void UpdateInventoryCount(int count, int maxCount)
        {
            countText.text = $"{count} / {maxCount}";
        }
        public void UpdateInventoryIcon(int index, Sprite sprite)
        {
            Image icon = inventoryGridParent.GetChild(index)
                .transform.Find("SlotBackground/ImageSlot").GetComponent<Image>();
           Color alpha = new Color(1, 1, 1, 1);
           icon.color = alpha;
           icon.sprite = sprite;
           
        }

        public void InitInventory()
        {
            Instantiate(inventoryIconPrefab, inventoryGridParent);
            //인벤토리 슬롯 초기 생성 
        }
        
        private void ShowCurrentItem()
        {
            //_currentWeaponSprite
        }
        
        
        private void Awake()
        {
            //Button Click Callback
            playerButton.onClick.AddListener(OnPlayerStatusMenu);
            weaponButton.onClick.AddListener(() => OpenInventory(ItemTypes.Weapon));
            equipmentButton.onClick.AddListener(() => OpenInventory(ItemTypes.Equipment));
            consumableButton.onClick.AddListener(() => OpenInventory(ItemTypes.Consumable));
            
         
        }

        // Start is called before the first frame update
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
            //PauseMenu 일시정지
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause(); //일시정지or재개 
            }

            //PlayerMenu
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            {
                TogglePlayerMenu();
            }

        }
    }
}
