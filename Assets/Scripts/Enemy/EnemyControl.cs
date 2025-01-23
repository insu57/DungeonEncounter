using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public enum EnemyStates
    {
        Idle = 0, Move, Attack, Damaged , Dead ,Global, JumpAttack
    }
    public class EnemyControl : MonoBehaviour
    {
        
        protected Dictionary<EnemyStates, State<EnemyControl>> States;
        private StateMachine<EnemyControl> _stateMachine;
        public EnemyStates CurrentState { private set; get; }
        
        public Animator EnemyAnimator { private set; get; }
        private string _currentAnimation;
        private GameObject _player;
        //protected PlayerManager PlayerManager;
        protected NavMeshAgent Agent;

        public bool IsMove { set; get; }
        public bool IsAttack { set; get; }
        public bool WasDamaged { set; get; }
        public bool IsDead { set; get; }
        protected bool InAttackDelay;

        public void ChangeState(EnemyStates newState)
        {
            CurrentState = newState;
            _stateMachine.ChangeState(States[newState]);
        }

        public void ChangeAnimation(string newAnimation, float crossFadeTime = 0.2f)//애니메이션 변경
        {
            if (_currentAnimation != newAnimation)
            {
                _currentAnimation = newAnimation;
                EnemyAnimator.CrossFade(newAnimation,crossFadeTime,-1,0);
            }
        }
        protected virtual IEnumerator AttackDelay(float duration)
        {
            InAttackDelay = true;
            yield return new WaitForSeconds(duration);
            InAttackDelay = false;
        }

        protected virtual void OnEnable()
        {
            //초기화
            IsAttack = false;
            IsMove = false;
            WasDamaged = false;
            IsDead = false;
            InAttackDelay = false;
            ChangeState(EnemyStates.Idle);
        }
        
        protected virtual void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            //PlayerManager = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
            Agent = GetComponent<NavMeshAgent>();
            EnemyAnimator = GetComponent<Animator>();

            States = new Dictionary<EnemyStates, State<EnemyControl>>()
            {
                { EnemyStates.Idle, new Idle() },
                { EnemyStates.Dead , new Dead()},
                { EnemyStates.Attack , new Attack()},
                { EnemyStates.Damaged, new Damaged() },
                { EnemyStates.Move , new Move()},
                { EnemyStates.Global , new StateGlobal()}
            };

            _stateMachine = new StateMachine<EnemyControl>();
            _stateMachine.Setup(this, States[EnemyStates.Idle]);
            _stateMachine.SetGlobalState(States[EnemyStates.Global]);
            
            IsAttack = false;
            IsMove = false;
            WasDamaged = false;
            IsDead = false;
            InAttackDelay = false;
        }

        protected virtual void EnemyAttack()
        {
            float animTime = EnemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (Agent.remainingDistance <= Agent.stoppingDistance && !Agent.pathPending) //Enemy 정지
            {
                Agent.isStopped = true;
                IsMove = false;
            
                if (InAttackDelay) return;
                IsAttack = true;
                if (animTime >= 1f && EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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
        
        protected virtual void Update()
        {
            _stateMachine.Execute();
            if (IsDead) return;
        
            Vector3 dir = _player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
            //lerp LookAt Player
        
            Agent.SetDestination(_player.transform.position); //NavMeshAgent 플레이어 추적 
        
            EnemyAttack();
        }
    }
}
