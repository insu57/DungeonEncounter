using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour //UI제어
{
    private PlayerManager _playerManager;
    private Image _healthBar;
    private float _healthBarWidth;
    private Image _energyBar;
    private float _energyBarWidth;
    private float _health;
    private float _maxHealth;
    private float _energy;
    private float _maxEnergy;
    
    private void Awake()
    {
        _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        _healthBar = transform.Find("Health").GetComponent<Image>();
        _healthBarWidth = _healthBar.rectTransform.sizeDelta.x;
        _energyBar = transform.Find("Energy").GetComponent<Image>();
        _energyBarWidth = _energyBar.rectTransform.sizeDelta.x;
        
    }

    private void Start()
    {
        _health = _playerManager.Health;
        _energy = _playerManager.Energy;
        _maxHealth = _playerManager.MaxHealth;
        _maxEnergy = _playerManager.MaxEnergy;
    }

    private void Update()
    {
        _health = _playerManager.Health;
        _energy = _playerManager.Energy;
        
        _healthBar.fillAmount = _health / _maxHealth;
        _energyBar.fillAmount = _energy / _maxEnergy;
    }
}
