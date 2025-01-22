using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class Stage1BossMeleeAttack : EnemyMeleeAttack
{
    protected override void Update()
    {
        float animTime = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);

        if (EnemyControl is Stage1BossControl stage1BossControl)
        {
            if (stage1BossControl.IsJumpAttack)
            {
                attackArea.enabled = true;
                trailRenderer.enabled = true;
            }
            else if (EnemyControl.IsAttack && AttackStartTime <= animTime && animTime <= AttackEndTime)
            {
                attackArea.enabled = true;
                trailRenderer.enabled = true;
            }
            else
            {
                attackArea.enabled = false;
                trailRenderer.Clear();
                trailRenderer.enabled = false;
            }
        }
        
        
        
    }
}
