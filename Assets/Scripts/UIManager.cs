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
    private PlayerManager _playerManager;
    private InventoryManager _inventoryManager;
    private PlayerUIView _playerUIView;
    private EnemySpawnManager _enemySpawnManager;
    private WorldUIView _worldUIView;
    
    private PlayerUIPresenter _playerUIPresenter;
    private InventoryUIPresenter _inventoryUIPresenter;
    

    public void InitEnemyHealthBar()
    {
        
    }
    
    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _playerUIView = GetComponent<PlayerUIView>();
        _worldUIView = GetComponent<WorldUIView>();
        _enemySpawnManager = FindObjectOfType<EnemySpawnManager>();


    }

    private void Start()
    {
        _playerUIPresenter = new PlayerUIPresenter(_playerManager, _playerUIView);
        _inventoryUIPresenter = new InventoryUIPresenter(_playerManager, _inventoryManager, _playerUIView);
        //수정필요...적 추가시 대응

        
    }
    

    private void OnDestroy()
    {
        _playerUIPresenter?.Dispose();
        _inventoryUIPresenter?.Dispose();
        //_inventoryUIPresenter?.Dispose();
        //
    }
}
