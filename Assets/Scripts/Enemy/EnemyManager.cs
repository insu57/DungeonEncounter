
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { Idle = 0, Move, Attack, Damaged ,Global }

public class EnemyManager : MonoBehaviour //적
{
    private Animator _animator;
    private State<EnemyManager>[] _states;
    private StateMachine<EnemyManager> _stateMachine;
    private GameObject _player;
    private NavMeshAgent _agent;
    [SerializeField] private GameObject target;

    private float _health;
    private float _maxHealth;
    private bool _isMove;
    private bool _isAttack;
    private float _height;
    private bool _wasDamaged;

    public EnemyStates CurrentState { private set; get; }
    public Animator EnemyAnimator { private set; get; }

    public float Health { set => _health = Mathf.Max(0, value); get => _health; }
    public float MaxHealth { set => _maxHealth = Mathf.Max(0, value); get => _maxHealth;}
    public bool IsMove { set => _isMove = value; get => _isMove; }
    public bool IsAttack { set => _isAttack = value; get => _isAttack; }
    public bool WasDamaged { set => _wasDamaged = value; get => _wasDamaged; }
    public float Height => _height;

    public void ChangeState(EnemyStates newState)
    {
        CurrentState = newState;
        _stateMachine.ChangeState(_states[(int)newState]);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        EnemyAnimator = _animator;
        _player = GameObject.FindGameObjectWithTag("Player");
        _agent = GetComponent<NavMeshAgent>();
        
        _states = new State<EnemyManager>[5];
        _states[(int)EnemyStates.Idle] = new EnemyAnimState.Idle();
        _states[(int)EnemyStates.Move] = new EnemyAnimState.Move();
        _states[(int)EnemyStates.Attack] = new EnemyAnimState.Attack();
        _states[(int)EnemyStates.Damaged] = new EnemyAnimState.Damaged();
        _states[(int)EnemyStates.Global] = new EnemyAnimState.StateGlobal();
        _stateMachine = new StateMachine<EnemyManager>();
        _stateMachine.Setup(this, _states[(int)EnemyStates.Idle]);
        _stateMachine.SetGlobalState(_states[(int)EnemyStates.Global]);

        _health = 100f;
        _maxHealth = 100f;
        _height = GetComponent<Collider>().bounds.size.y;
        _isAttack = false;
        _isMove = false;
        _wasDamaged = false;
    }

    private void Update()
    {
        _stateMachine.Execute();
        
        _agent.SetDestination(_player.transform.position); //NavMeshAgent 플레이어 추적 
        
        if (_agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending) //Enemy 정지
        {
            _agent.isStopped = true;
            _isMove = false;
            _isAttack = true;
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
            PlayerWeapon playerWeapon = other.GetComponent<PlayerWeapon>();
            _wasDamaged = true;
            _health -= playerWeapon.Damage;
            Debug.Log("Enemy Health: " + _health);
            StartCoroutine(Damaged(0.5f));
        }
    }

    private IEnumerator Damaged(float duration)
    {
        yield return new WaitForSeconds(duration);
        _wasDamaged = false;
    }

}
