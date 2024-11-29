using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIView : MonoBehaviour
    {
        private GameManager _gameManager;
        private Canvas _canvasMain;
        //Game Menu
        private GameObject _pauseMenu;
        private Button _resumeButton;
    
        //Player Info
        private GameObject _playerInfo;//직업 관련 추가 필요
        private Image _playerHealthBar;
        private Image _playerEnergyBar;
        
    
        //PlayerMenu...Inventory
        private GameObject _playerMenu;
        private GameObject _playerMenuMain;
        private GameObject _playerStatus;
        private GameObject _inventory;
        private Button _playerButton;
        private Button _weaponButton;
        private Button _equipmentButton;
        private Button _consumableButton;
        
        private TextMeshProUGUI _jobText;
        private TextMeshProUGUI _healthText;
        private TextMeshProUGUI _energyText;
        private TextMeshProUGUI _attackText;
        private TextMeshProUGUI _defenseText;
        private Image _currentWeaponImg;
        private Image _currentEquipmentImg;
        private Image _currentQuick1Img;
        private Image _currentQuick2Img;
       
        public event Action OnWeaponChanged;
        public event Action OnEquipmentChanged;
        public event Action<int> OnItemQuickChanged;
        public event Action OnShowIcon; //icon? data?
        
        
        private void TogglePause()
        {
            _gameManager.TogglePause();
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        }

        private void TogglePlayerMenu()
        {
            _playerMenu.SetActive(!_playerMenu.activeSelf);
        }
        
        public void UpdatePlayerHealthBar(float health, float maxHealth)
        {
            _playerHealthBar.fillAmount = health / maxHealth;
            _healthText.text = $"{health}/{maxHealth}";
            Debug.Log($"{health}/{maxHealth}");
        }

        public void UpdatePlayerEnergyBar(float energy, float maxEnergy)
        {
            _playerEnergyBar.fillAmount = energy / maxEnergy;
            _energyText.text = $"{energy}/{maxEnergy}";
        }
        //플레이어 정보 창일때만 호출하게 수정필요
        public void UpdatePlayerAttackValue(float attackValue)
        {
            _attackText.text = $"{attackValue}";
        }

        public void UpdatePlayerDefenseValue(float defenseValue)
        {
            _defenseText.text = $"{defenseValue}";
        }

        private void OnPlayerStatusMenu()
        {
            _inventory.SetActive(false);
            
        }

        private void OnWeaponInventory()
        {
            _inventory.SetActive(true);
        }

        private void OnEquipmentInventory()
        {
            _inventory.SetActive(true);
        }

        private void OnConsumableInventory()
        {
            _inventory.SetActive(true);
        }

        public void UpdateCurrentWeapon(Sprite sprite)
        {
            _currentWeaponImg.sprite = sprite;
            Color alpha = new Color(1,1,1, 1);
            _currentWeaponImg.color = alpha;
        }
        private void ShowCurrentItem()
        {
            //_currentWeaponSprite
        }
        
        
        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _canvasMain = GameObject.Find("CanvasMain").GetComponent<Canvas>();
        
            //PauseMenu
            _pauseMenu = _canvasMain.transform.Find("PauseMenu").gameObject;
            _resumeButton = _pauseMenu.transform.Find("ResumeButton").GetComponent<Button>();
            _resumeButton.onClick.AddListener(TogglePause);//일시정지 재개
        
            //PlayerMenu
            //Buttons
            _playerMenu = _canvasMain.transform.Find("PlayerMenu").gameObject;
            _playerButton = _playerMenu.transform.Find("Buttons/PlayerButton").gameObject.GetComponent<Button>();
            _weaponButton = _playerMenu.transform.Find("Buttons/WeaponButton").GetComponent<Button>();
            _equipmentButton = _playerMenu.transform.Find("Buttons/EquipmentButton").GetComponent<Button>();
            _consumableButton = _playerMenu.transform.Find("Buttons/ConsumableButton").GetComponent<Button>();
            _playerButton.onClick.AddListener(OnPlayerStatusMenu);
            _weaponButton.onClick.AddListener(OnWeaponInventory);
            _equipmentButton.onClick.AddListener(OnEquipmentInventory);
            _consumableButton.onClick.AddListener(OnConsumableInventory);
            //Main
            _playerMenuMain = _playerMenu.transform.Find("Main").gameObject;
            _playerStatus = _playerMenuMain.transform.Find("PlayerStatus").gameObject;
            _inventory = _playerMenuMain.transform.Find("Inventory").gameObject;
            //...Player Status
         
            _jobText = _playerStatus.transform.Find("JobText").GetComponent<TextMeshProUGUI>();
            _healthText = _playerStatus.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
            _energyText = _playerStatus.transform.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            _attackText = _playerStatus.transform.Find("AttackText").GetComponent<TextMeshProUGUI>();
            _defenseText = _playerStatus.transform.Find("DefenseText").GetComponent<TextMeshProUGUI>();
            _currentWeaponImg = _playerStatus.transform.Find("CurrentWeaponIcon").GetComponent<Image>();
        
            //PlayerInfo
            _playerInfo = _canvasMain.transform.Find("PlayerInfo").gameObject;
            _playerHealthBar = _playerInfo.transform.Find("Health").GetComponent<Image>();
            _playerEnergyBar = _playerInfo.transform.Find("Energy").GetComponent<Image>();
         
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
