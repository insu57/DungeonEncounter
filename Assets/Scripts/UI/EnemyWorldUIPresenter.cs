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
            _enemy.OnHealthChanged += HandleEnemyHealthChange;
        }
        
        private void HandleEnemyHealthChange(float health, float maxHealth)
        {
            _healthBar.UpdateHealthBar(health, maxHealth);
        }

        public void Dispose()
        {
            _enemy.OnHealthChanged -= HandleEnemyHealthChange;
        }
    }
}
