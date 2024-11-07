using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
   private EnemyManager _enemyManager;
   private Image _healthBar;

   public void Init(EnemyManager enemyManager)
   {
      _enemyManager = enemyManager;
      _healthBar = transform.Find("Health").GetComponent<Image>();
   }
   
   public void UpdateHealthBar()
   {
      var pos = new Vector3(_enemyManager.transform.position.x,
         _enemyManager.Height + 0.2f, _enemyManager.transform.position.z);
      transform.position = pos; //위치 갱신
      
      _healthBar.fillAmount = _enemyManager.Health / _enemyManager.MaxHealth; //체력바 갱신
   }


}
