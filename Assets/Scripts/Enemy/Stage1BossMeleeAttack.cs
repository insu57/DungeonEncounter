using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class Stage1BossMeleeAttack : EnemyMeleeAttack
{
    private Vector3 _originalSize;
    
    public void JumpAttackArea()
    {
        //attackArea.size
        if (attackArea is not BoxCollider boxCollider) return;
        if (EnemyControl is Stage1BossControl { IsJumpAttack: true })
        {
            boxCollider.size = _originalSize * 2.5f;
        }
        else
        {
            boxCollider.size = _originalSize;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (attackArea is BoxCollider boxCollider)
        {
            _originalSize = boxCollider.size;
        }
    }
    
    protected override void Update()
    {
        float animTime = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);

        if (EnemyControl is Stage1BossControl stage1BossControl)
        {
            if (stage1BossControl.IsJumpAttack)
            {
                attackArea.enabled = true;
                trailRenderer.enabled = false;
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
