using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using UnityEngine;

namespace UI
{
    public class EnemyWorldUIPresenter
    {
        private readonly EnemySpawnManager _spawnManager;
        private readonly WorldUIView _view;
        private readonly EnemyManager _enemy;
        private readonly EnemyHealthBar _healthBar;

        public EnemyWorldUIPresenter(EnemyManager enemy, EnemyHealthBar healthBar)
        {
            _enemy = enemy;
            _healthBar = healthBar;
            _healthBar.UpdateHealthBar(_enemy.GetStat(EnemyStatTypes.Health), 
                _enemy.GetStat(EnemyStatTypes.MaxHealth));//체력바 초기화
            _enemy.OnHealthChanged += HandleEnemyHealthChange;
            _enemy.OnDeath += HandleEnemyDeath;
        }
        
        private void HandleEnemyHealthChange(float health, float maxHealth)
        {
            _healthBar.UpdateHealthBar(health, maxHealth); //체력 변경시 업데이트
        }

        private void HandleEnemyDeath()
        {
            ReturnHealthBar();//사망시 ReturnToPool
        }

        public void ReturnHealthBar()
        {
            _healthBar.transform.SetParent(null);
            ObjectPoolingManager.Instance.ReturnToPool(PoolKeys.HealthBar, _healthBar.gameObject);
        }
        
        public void Dispose()
        {
            _enemy.OnHealthChanged -= HandleEnemyHealthChange;
            _enemy.OnDeath -= HandleEnemyDeath;
        }
    }
}
