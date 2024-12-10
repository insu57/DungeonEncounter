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
    
    private PlayerUIPresenter _playerUIPresenter;
    private InventoryUIPresenter _inventoryUIPresenter;
    
    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _playerUIView = GetComponent<PlayerUIView>();
        _inventoryUIView = GetComponent<InventoryUIView>();
    }

    private void Start()
    {
        _playerUIPresenter = new PlayerUIPresenter(_playerManager, _playerUIView);
        _inventoryUIPresenter = new InventoryUIPresenter(_playerManager, _inventoryManager, _inventoryUIView);
    }
    

    private void OnDestroy()
    {
        _playerUIPresenter?.Dispose();
        _inventoryUIPresenter?.Dispose();
        
    }
}
