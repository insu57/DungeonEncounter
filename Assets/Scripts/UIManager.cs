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
    
    private Canvas _canvasFloat;
    //적 위에 떠있는 체력바...프리팹사용?
    private GameObject _enemyHealthPrefab;
    private Image _enemyHealthBar;
    private Image _enemyEnergyBar;
    private float _enemyHealth;
    private float _enemyMaxHealth;

    private GameObject[] _enemy;
    
    private void Awake()
    {
        _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        _playerHealthBar = transform.Find("Health").GetComponent<Image>();
        _playerEnergyBar = transform.Find("Energy").GetComponent<Image>();

        _enemy = GameObject.FindGameObjectsWithTag("Enemy");
        _enemyHealthPrefab = Resources.Load<GameObject>("Prefabs/EnemyHealth");
        
    }

    private void Start()
    {
        _playerHealth = _playerManager.Health;
        _playerEnergy = _playerManager.Energy;
        _playerMaxHealth = _playerManager.MaxHealth;
        _playerMaxEnergy = _playerManager.MaxEnergy;

        foreach (GameObject e in _enemy)
        {
            Vector3 pos = e.GetComponent<Transform>().position;
            pos.y += 1f;
            Instantiate(_enemyHealthPrefab, e.transform.position, e.transform.rotation);
        }
        
        
    }

    private void Update()
    {
        _playerHealth = _playerManager.Health;
        _playerEnergy = _playerManager.Energy;
        _playerHealthBar.fillAmount = _playerHealth / _playerMaxHealth;
        _playerEnergyBar.fillAmount = _playerEnergy / _playerMaxEnergy;
        //플레이어 정보 표시
    }
}
