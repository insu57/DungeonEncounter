using System;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Enemy
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject enemy1;
        [SerializeField] private GameObject enemy2;
        [SerializeField] private Transform pos1;
        [SerializeField] private Transform pos2;
        private void SpawnEnemy(GameObject enemy, Transform spawnPoint)
        {
            Instantiate(enemy, spawnPoint);
        }

        public void Start()
        {
            SpawnEnemy(enemy1, pos1);
            SpawnEnemy(enemy2, pos2);
        }
    }
}
