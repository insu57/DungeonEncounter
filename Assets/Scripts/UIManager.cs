using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour //UI제어
{
    private GameManager _gameManager;
    private Canvas _canvasMain;
    //Game Menu
    private GameObject _pauseMenu;
    private Button _resumeButton;
    
    //Player Info
    private PlayerManager _playerManager;
    private GameObject _playerInfo;//직업 관련 추가 필요
    private Image _playerHealthBar;
    private Image _playerEnergyBar;
    private float _playerHealth;
    private float _playerMaxHealth;
    private float _playerEnergy;
    private float _playerMaxEnergy;
    
    //PlayerMenu...Inventory
    private GameObject _playerMenu;
    private Button _playerButton;
    private Button _weaponButton;
    private Button _equipmentButton;
    private Button _consumableButton;
    
    //Enemy Info(Health)
    private EnemyManager[] _enemyManagers;
    private Canvas _canvasFloat;
    private Camera _mainCamera;
    private GameObject _enemyHealthPrefab;
    private Dictionary<EnemyManager, EnemyHealthBar> _enemyHealthUI; //적 체력 정보

    private void TogglePause()
    {
        if (!_gameManager.GamePaused)
        {
            _gameManager.GamePaused = true;
            _pauseMenu.SetActive(true);
        }
        else
        {
            _gameManager.GamePaused = false;
            _pauseMenu.SetActive(false);
        }
    }

    private void TogglePlayerMenu()
    {
        _playerMenu.SetActive(!_playerMenu.activeSelf);
    }
    
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _canvasMain = GameObject.Find("CanvasMain").GetComponent<Canvas>();
        _canvasFloat = GameObject.Find("CanvasFloat").GetComponent<Canvas>();
        _mainCamera = Camera.main;
        _canvasFloat.worldCamera = _mainCamera;
        
        //PauseMenu
        _pauseMenu = _canvasMain.transform.Find("PauseMenu").gameObject;
        _resumeButton = _pauseMenu.transform.Find("ResumeButton").GetComponent<Button>();
        _resumeButton.onClick.AddListener(TogglePause);//일시정지 재개
        
        //PlayerMenu
        _playerMenu = _canvasMain.transform.Find("PlayerMenu").gameObject;
        _playerButton = _playerMenu.transform.Find("PlayerButton").GetComponent<Button>();
        _weaponButton = _playerMenu.transform.Find("WeaponButton").GetComponent<Button>();
        _equipmentButton = _playerMenu.transform.Find("EquipmentButton").GetComponent<Button>();
        _consumableButton = _playerMenu.transform.Find("ConsumableButton").GetComponent<Button>();
        
        //PlayerInfo
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerInfo = _canvasMain.transform.Find("PlayerInfo").gameObject;
        _playerHealthBar = _playerInfo.transform.Find("Health").GetComponent<Image>();
        _playerEnergyBar = _playerInfo.transform.Find("Energy").GetComponent<Image>();
        
        //EnemyInfo
        _enemyHealthPrefab = Resources.Load<GameObject>("Prefabs/EnemyHealth");
        _enemyHealthUI = new Dictionary<EnemyManager, EnemyHealthBar>();
        _enemyManagers = FindObjectsOfType<EnemyManager>();
    }

    private void Start()
    {
        foreach (var enemyManager in _enemyManagers)
        {
            var enemyHealthUI = Instantiate(_enemyHealthPrefab, _canvasFloat.transform); //적 체력 UI 생성
            var enemyHealthBar = enemyHealthUI.GetComponent<EnemyHealthBar>();
            enemyHealthBar.Init(enemyManager);
            _enemyHealthUI[enemyManager] = enemyHealthBar;
        }
        //수정필요...적 추가시 대응
    }

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
        
        //플레이어 정보 표시
        _playerHealth = _playerManager.Health;
        _playerEnergy = _playerManager.Energy;
        _playerMaxHealth = _playerManager.MaxHealth;
        _playerMaxEnergy = _playerManager.MaxEnergy;//플레이어 상태정보
        _playerHealthBar.fillAmount = _playerHealth / _playerMaxHealth;
        _playerEnergyBar.fillAmount = _playerEnergy / _playerMaxEnergy;
        
        foreach (var (_, enemyHealthBar) in _enemyHealthUI)
        {
            enemyHealthBar.UpdateHealthBar(); //적 체력바 업데이트
        }
    }
    
    
}
