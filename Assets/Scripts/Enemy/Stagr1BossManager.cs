using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class Stagr1BossManager : EnemyManager
{
    protected override void EnemyDeath()
    {
        base.EnemyDeath();
        var bossDrop = Random.Range(0, 3);//0~2개 추가 드랍
        for (int i = 0; i < bossDrop; i++)
        {
            DropItem();
        }
    }
}
