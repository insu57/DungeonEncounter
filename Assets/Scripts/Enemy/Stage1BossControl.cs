using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;


public class Stage1BossAttack : Attack
{
    public override void Enter(EnemyControl enemy)
    {
        enemy.ChangeAnimation(enemy is Stage1BossControl { IsJumpAttack: true } ? "JumpAttack" : "Attack");
    }
}

public class Stage1BossControl : EnemyControl
{
    [SerializeField] private GameObject jumpAttackEffect;
    private int _attackCount = 0;
    private int _randomStack;
    public bool IsJumpAttack { private set; get; }
    protected override void EnemyAttack()
    {
        float animTime = EnemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        
        
        if (Agent.remainingDistance <= Agent.stoppingDistance && !Agent.pathPending) //Enemy 정지
        {
            Agent.isStopped = true;
            IsMove = false;
            
            if (InAttackDelay) return;
            
            if (_attackCount == 0)
            {
                _randomStack = Random.Range(3, 6);//3~5
            }
            
            _attackCount++;
            
            if (_attackCount == _randomStack)
            {
                IsJumpAttack = true;
                _attackCount = 0;
                jumpAttackEffect.SetActive(true);
                Debug.Log("Jump Attack"); //이펙트 이상
            }
            else
            {
                if (IsJumpAttack)
                {
                    IsJumpAttack = false;
                    jumpAttackEffect.SetActive(false);
                }
            }
            
            IsAttack = true;
            bool isAttack = EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
                            EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack");
            
            if (animTime >= 1f && isAttack)
            {
                IsAttack = false;
                StartCoroutine(AttackDelay(1f));//1f delay
            }
            
        }
        else
        {
            Agent.isStopped = false;
            IsMove = true;
            IsAttack = false;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        States[EnemyStates.Attack] = new Stage1BossAttack();
    }
}
