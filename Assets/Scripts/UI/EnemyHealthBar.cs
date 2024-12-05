using System;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
   public class EnemyHealthBar : MonoBehaviour
   {
      private EnemyManager _enemyManager;
      [SerializeField]private Image healthBar;

      public void Init(EnemyManager enemyManager)
      {
         _enemyManager = enemyManager;
      }

      public void UpdateHealthBar(float health, float maxHealth)
      {
         healthBar.fillAmount = health / maxHealth;
      }

      public void Update()
      {
         var pos = new Vector3(_enemyManager.transform.position.x,
            _enemyManager.Height + 0.2f, _enemyManager.transform.position.z);
         transform.position = pos; //위치 갱신
      }
   }
}
