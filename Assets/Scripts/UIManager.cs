using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour //UI제어
{
    private PlayerManager _playerManager;
    private Canvas _canvasMain;
    private Image _playerHealthBar;
    private Image _playerEnergyBar;
    private float _playerHealth;
    private float _playerMaxHealth;
    private float _playerEnergy;
    private float _playerMaxEnergy;
    
    private EnemyManager[] _enemyManagers;
    private Canvas _canvasFloat;
    private Camera _mainCamera;
    private GameObject _enemyHealthPrefab;
    private Dictionary<EnemyManager, EnemyHealthBar> _enemyHealthUI; //적 체력 정보
    
    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerHealthBar = transform.Find("Health").GetComponent<Image>();
        _playerEnergyBar = transform.Find("Energy").GetComponent<Image>();
        
        _enemyHealthPrefab = Resources.Load<GameObject>("Prefabs/EnemyHealth");
        _canvasFloat = GameObject.Find("CanvasFloat").GetComponent<Canvas>();
        _mainCamera = Camera.main;
        _canvasFloat.worldCamera = _mainCamera;
        
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
    }

    private void Update()
    {
        _playerHealth = _playerManager.Health;//플레이어 상태정보
        _playerEnergy = _playerManager.Energy;
        _playerMaxHealth = _playerManager.MaxHealth;
        _playerMaxEnergy = _playerManager.MaxEnergy;
        
        _playerHealthBar.fillAmount = _playerHealth / _playerMaxHealth;
        _playerEnergyBar.fillAmount = _playerEnergy / _playerMaxEnergy;
        //플레이어 정보 표시
        
        foreach (var (_, enemyHealthBar) in _enemyHealthUI)
        {
            enemyHealthBar.UpdateHealthBar(); //적 체력바 업데이트
        }
    }
}
