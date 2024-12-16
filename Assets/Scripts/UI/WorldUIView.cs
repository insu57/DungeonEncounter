using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class WorldUIView : MonoBehaviour
    {
        [SerializeField] private Canvas canvasFloat;
        private Camera _mainCamera;
        [SerializeField] private GameObject enemyHealthPrefab;
        private Dictionary<EnemyManager, EnemyHealthBar> _enemyHealthUI; //적 체력 정보
        private Image _healthBar;
        private EnemyWorldUIPresenter _enemyWorldUIPresenter;
        public EnemyHealthBar InitEnemyHealthBar(EnemyManager enemyManager)
        {
            
            GameObject enemyHealth =  Instantiate(enemyHealthPrefab, canvasFloat.transform);
            EnemyHealthBar healthBar = enemyHealth.GetComponent<EnemyHealthBar>();
            healthBar.Init(enemyManager);
            return healthBar;
        }
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            canvasFloat.worldCamera = _mainCamera;
            
        }

    }
}
