using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public enum EnemyStates
    {
        Idle = 0, Move, Attack, Damaged , Dead ,Global
    }
    public class EnemyControl : MonoBehaviour
    {
        
        private State<EnemyControl>[] _states;
        private StateMachine<EnemyControl> _stateMachine;
        public EnemyStates CurrentState { private set; get; }
        
        public Animator EnemyAnimator { private set; get; }
        private string _currentAnimation;
        private GameObject _player;
        private PlayerManager _playerManager;
        private NavMeshAgent _agent;

        public bool IsMove { set; get; }
        public bool IsAttack { set; get; }
        public bool WasDamaged { set; get; }
        public bool IsDead { set; get; }
        private bool InAttackDelay { set; get; }

        public void ChangeState(EnemyStates newState)
        {
            CurrentState = newState;
            _stateMachine.ChangeState(_states[(int)newState]);
        }

        public void ChangeAnimation(string newAnimation, float crossFadeTime = 0.2f)//애니메이션 변경
        {
            if (_currentAnimation != newAnimation)
            {
                _currentAnimation = newAnimation;
                EnemyAnimator.CrossFade(newAnimation,crossFadeTime,-1,0);
            }
        }
        private IEnumerator AttackDelay(float duration)
        {
            InAttackDelay = true;
            yield return new WaitForSeconds(duration);
            InAttackDelay = false;
        }

        private void OnEnable()
        {
            ChangeState(EnemyStates.Idle);
        }

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerManager = _player.GetComponent<PlayerManager>();
            _agent = GetComponent<NavMeshAgent>();
            EnemyAnimator = GetComponent<Animator>();
            
            _states = new State<EnemyControl>[6];
            _states[(int)EnemyStates.Idle] = new Idle();
            _states[(int)EnemyStates.Move] = new Move();
            _states[(int)EnemyStates.Attack] = new Attack();
            _states[(int)EnemyStates.Damaged] = new Damaged();
            _states[(int)EnemyStates.Dead] = new Dead();
            _states[(int)EnemyStates.Global] = new StateGlobal();
            
            _stateMachine = new StateMachine<EnemyControl>();
            _stateMachine.Setup(this, _states[(int)EnemyStates.Idle]);
            _stateMachine.SetGlobalState(_states[(int)EnemyStates.Global]);
            
            IsAttack = false;
            IsMove = false;
            WasDamaged = false;
            IsDead = false;
            InAttackDelay = false;
        }

        private void Update()
        {
            _stateMachine.Execute();
            if (IsDead) return;
        
            Vector3 dir = _player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
            //lerp LookAt Player
        
            _agent.SetDestination(_player.transform.position); //NavMeshAgent 플레이어 추적 
        
            float animTime = EnemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending) //Enemy 정지
            {
                _agent.isStopped = true;
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
                _agent.isStopped = false;
                IsMove = true;
                IsAttack = false;
            }
        }
    }
}
