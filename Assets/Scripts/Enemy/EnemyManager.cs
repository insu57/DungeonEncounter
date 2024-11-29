using System.Collections;
using DG.Tweening;
using Player;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public enum EnemyStates { Idle = 0, Move, Attack, Damaged , Dead ,Global }

    public class EnemyManager : MonoBehaviour //적
    {
        private GameManager _gameManager;
        private Animator _animator;
        private State<EnemyManager>[] _states;
        private StateMachine<EnemyManager> _stateMachine;
        private GameObject _player;
        private PlayerManager _playerManager;
        private Tween _lookAtTween;
        private NavMeshAgent _agent;
        [SerializeField] private EnemyData data;
        private EnemyDropTable _dropTable;
        private string _type;
        private float _health;
        private float _maxHealth;
        private float _damage;
    
        private bool _isMove;
        private bool _isAttack;
        private float _height;
        private bool _wasDamaged;
        private bool _isDead;
        private bool _inAttackDelay;
        
        public EnemyStates CurrentState { private set; get; }
        public Animator EnemyAnimator { private set; get; }

        public EnemyData Data => data;
        public float Health { set => _health = Mathf.Max(0, value); get => _health; }
        public float MaxHealth { set => _maxHealth = Mathf.Max(0, value); get => _maxHealth;}
        public float Height => _height;
        public float Damage => _damage;
        public bool IsMove { set => _isMove = value; get => _isMove; }
        public bool IsAttack { set => _isAttack = value; get => _isAttack; }
        public bool WasDamaged { set => _wasDamaged = value; get => _wasDamaged; }
        public bool IsDead { set => _isDead = value; get => _isDead; }
        public bool InAttackDelay { set => _inAttackDelay = value; get => _inAttackDelay; }
    
        public void ChangeState(EnemyStates newState)
        {
            CurrentState = newState;
            _stateMachine.ChangeState(_states[(int)newState]);
        }

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _animator = GetComponent<Animator>();
            EnemyAnimator = _animator;
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerManager = _player.GetComponent<PlayerManager>();
            _agent = GetComponent<NavMeshAgent>();
            _dropTable = data.DropTable;
        
            _states = new State<EnemyManager>[6];
            _states[(int)EnemyStates.Idle] = new Idle();
            _states[(int)EnemyStates.Move] = new Move();
            _states[(int)EnemyStates.Attack] = new Attack();
            _states[(int)EnemyStates.Damaged] = new Damaged();
            _states[(int)EnemyStates.Dead] = new Dead();
            _states[(int)EnemyStates.Global] = new StateGlobal();
            _stateMachine = new StateMachine<EnemyManager>();
            _stateMachine.Setup(this, _states[(int)EnemyStates.Idle]);
            _stateMachine.SetGlobalState(_states[(int)EnemyStates.Global]);

            _type = data.Type;
            _maxHealth = data.MaxHealth;
            _health = _maxHealth;
            _damage = data.Damage;
            _height = GetComponent<Collider>().bounds.size.y;
            _isAttack = false;
            _isMove = false;
            _wasDamaged = false;
            _isDead = false;
            _inAttackDelay = false;
        
        }
    
        private void Update()
        {
            _stateMachine.Execute();
            if (_isDead) return;
        
            Vector3 dir = _player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
            //lerp LookAt Player
        
            _agent.SetDestination(_player.transform.position); //NavMeshAgent 플레이어 추적 
        
            float animTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending) //Enemy 정지
            {
                _agent.isStopped = true;
                _isMove = false;
            
                //StartCoroutine(AttackDelay(1f));
            
                if (_inAttackDelay) return;
                _isAttack = true;
                if (animTime >= 1f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    _isAttack = false;
                    StartCoroutine(AttackDelay(1f));//1f delay
                }
            
            }
            else
            {
                _agent.isStopped = false;
                _isMove = true;
                _isAttack = false;
            }
        
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttack") && _wasDamaged == false)
            {
                //ItemData playerWeapon = other.GetComponent<GetItemData>().ItemData; //원거리 추가 필요
                //player기준...
                float damage = _playerManager.GetStat(PlayerStatTypes.AttackValue);
                _health -= damage;
                //test
                Debug.Log(data.EnemyName + " Health: "+_health);
                if (_health <= 0)
                {
                    _isAttack = false;
                    _isMove = false;
                    _isDead = true;
                }
            
                StartCoroutine(Damaged(0.5f));
            }
        }

        private IEnumerator Damaged(float duration)
        {
            _wasDamaged = true;
            yield return new WaitForSeconds(duration);
            _wasDamaged = false;
        }

        private IEnumerator AttackDelay(float duration)
        {
            _inAttackDelay = true;
            yield return new WaitForSeconds(duration);
            _inAttackDelay = false;
        }
    
    
    }
}