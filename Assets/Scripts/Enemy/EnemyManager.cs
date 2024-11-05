using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum EnemyStates { Idle = 0, Move, Attack, Global }
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
    
    public EnemyStates CurrentState { set; get; }
    public Animator EnemyAnimator { private set; get; }
    public float Health { set => _health = Mathf.Max(0,value); get => _health; }
    public float MaxHealth { set => _maxHealth = Mathf.Max(0,value); get => _maxHealth; }
    public bool IsMove { set => _isMove = value; get => _isMove; }
    public bool IsAttack { set => _isAttack = value ; get => _isAttack; }
    
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
        
        _states = new State<EnemyManager>[4];
        _states[(int)EnemyStates.Idle] = new EnemyAnimState.Idle();
        _states[(int)EnemyStates.Move] = new EnemyAnimState.Move();
        _states[(int)EnemyStates.Attack] = new EnemyAnimState.Attack();
        _states[(int)EnemyStates.Global] = new EnemyAnimState.StateGlobal();
        _stateMachine = new StateMachine<EnemyManager>();
        _stateMachine.Setup(this, _states[(int)EnemyStates.Idle]);
        _stateMachine.SetGlobalState(_states[(int)EnemyStates.Global]);

        _health = 100f;
        _maxHealth = 100f;

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
}
