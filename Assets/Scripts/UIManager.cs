using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour //UI제어
{
    private PlayerUIPresenter _playerUIPresenter;
    private InventoryUIPresenter _inventoryUIPresenter;
    private PlayerManager _playerManager;
    private InventoryManager _inventoryManager;
    private PlayerUIView _playerUIView;
    
    private GameManager _gameManager;
    
    
    //Enemy Info(Health)
    private EnemyManager[] _enemyManagers;
    private Canvas _canvasFloat;
    private Camera _mainCamera;
    private GameObject _enemyHealthPrefab;
    private Dictionary<EnemyManager, EnemyHealthBar> _enemyHealthUI; //적 체력 정보

   
    
    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _playerUIView = GetComponent<PlayerUIView>();
        _gameManager = FindObjectOfType<GameManager>();
        
        _canvasFloat = GameObject.Find("CanvasFloat").GetComponent<Canvas>();
        _mainCamera = Camera.main;
        _canvasFloat.worldCamera = _mainCamera;
        
        
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

        _playerUIPresenter = new PlayerUIPresenter(_playerManager, _playerUIView);
        _inventoryUIPresenter = new InventoryUIPresenter(_playerManager, _inventoryManager, _playerUIView);
    }
    
    private void Update()
    {
        
        
        foreach (var (_, enemyHealthBar) in _enemyHealthUI)
        {
            enemyHealthBar.UpdateHealthBar(); //적 체력바 업데이트
        }
    }

    private void OnDestroy()
    {
        _playerUIPresenter?.Dispose();
    }
}
