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
    private InventoryUIView _inventoryUIView;
    private WorldUIView _worldUIView;
    
    private PlayerUIPresenter _playerUIPresenter;
    private InventoryUIPresenter _inventoryUIPresenter;
    private PlayerWorldUIPresenter _playerWorldUIPresenter;

    private void Awake()
    {
        //base.Awake();
        _playerManager = FindObjectOfType<PlayerManager>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _playerUIView = GetComponent<PlayerUIView>();
        _inventoryUIView = GetComponent<InventoryUIView>();
        _worldUIView = GetComponent<WorldUIView>();
    }

    private void Start()
    {
        _playerUIPresenter = new PlayerUIPresenter(_playerManager, _playerUIView);
        _inventoryUIPresenter = new InventoryUIPresenter(_playerManager, _inventoryManager, _inventoryUIView);
        _playerWorldUIPresenter = new PlayerWorldUIPresenter(_playerManager, _worldUIView);
    }
    

    private void OnDestroy()
    {
        _playerUIPresenter?.Dispose();
        _inventoryUIPresenter?.Dispose();
        _playerWorldUIPresenter?.Dispose();
    }
}
