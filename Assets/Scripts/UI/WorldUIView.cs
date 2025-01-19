using System;
using System.Collections.Generic;
using Enemy;
using TMPro;
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
        
        [SerializeField] private GameObject floatingKeyPrefab;
        private TextMeshPro _floatingKeyText;
        public EnemyHealthBar InitEnemyHealthBar(EnemyManager enemyManager)
        {
            GameObject enemyHealth = ObjectPoolingManager.Instance.GetObjectFromPool(PoolKeys.HealthBar,canvasFloat.transform.position, Quaternion.identity);
            enemyHealth.transform.SetParent(canvasFloat.transform);//부모설정 CanvasFloat(WorldCanvas)
            EnemyHealthBar healthBar = enemyHealth.GetComponent<EnemyHealthBar>();
            healthBar.Init(enemyManager);//초기화
            return healthBar;
        }

        public void SetFloatingKeyText(string text, Vector3 position)
        {
            floatingKeyPrefab.SetActive(true);
            _floatingKeyText.text = text;
            floatingKeyPrefab.transform.position = position;
        }

        public void InactivateFloatingKey()
        {
            floatingKeyPrefab.SetActive(false);
        }

        private void OnEnable()
        {
            _mainCamera = Camera.main;
            canvasFloat.worldCamera = _mainCamera;
        }

        private void Awake()
        {
            _floatingKeyText = floatingKeyPrefab.GetComponentInChildren<TextMeshPro>();
        }

    }
}
