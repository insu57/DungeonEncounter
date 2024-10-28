using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem, Global }

public class PlayerManager : MonoBehaviour //진행중 상태머신?????? 애니메이션->StateMachine 플레이어 Move 분리????
{
    private int health;
    private int energy;
    private int dodgeGauge;
    private bool isMove;
    private bool isAttack;
    private Vector3 moveVec;

    private CharacterMove charMove;
    private float hAxis; //x축
    private float vAxis; //z축
    //private Vector3 moveVec;
    public Animator animator;

    private State<PlayerManager>[] states;
    private StateMachine<PlayerManager> stateMachine;

    public PlayerStates CurrentState { private set; get; }

    public int Health
    {
        set => health = Mathf.Max(0, value);
        get => health;
    }
    public int Energy
    {
        set => energy = Mathf.Max(0, value);
        get => energy;
    }
    public int DodgeGauge
    {
        set => dodgeGauge = Mathf.Max(0, value);
        get => dodgeGauge;
    }

    public  bool IsMove
    {
        set => isMove = value;
        get => isMove;
    }
    public bool IsAttack
    {
        set => isAttack = value;
        get => isAttack;
    }

    public Vector3 MoveVec
    {
        set => moveVec = value;
        get => moveVec;
    }


    public void ChangeState(PlayerStates newState)
    {
        stateMachine.ChangeState(states[(int)newState]);
    }
    public void RevertToPreviousState()
    {
        stateMachine.RevertToPreviousState();
    }

    void Awake()
    {
        charMove = GetComponent<CharacterMove>();
        animator = GetComponentInChildren<Animator>();

        states = new State<PlayerManager>[6];
        states[(int)PlayerStates.Idle] = new PlayerAnimState.Idle();
        states[(int)PlayerStates.Run] = new PlayerAnimState.Run();
        states[(int)PlayerStates.Attack] = new PlayerAnimState.Attack();
        states[(int)PlayerStates.Dodge] = new PlayerAnimState.Dodge();
        states[(int)PlayerStates.UseItem] = new PlayerAnimState.UseItem();
        states[(int)PlayerStates.Global] =  new PlayerAnimState.StateGlobal();

        stateMachine = new StateMachine<PlayerManager>();
        stateMachine.Setup(this, states[(int)PlayerStates.Idle]); //초기 애니메이션 상태 Idle
        stateMachine.SetGlobalState(states[(int)PlayerStates.Global]);

        health = 100;
        energy = 100;
        dodgeGauge = 100;
        isMove = false;
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Execute();

        //x,z축 방향 이동
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (!isAttack)//공격등 행동시 이동x 진행중
        {
            charMove.MoveTo(moveVec);
            transform.LookAt(transform.position + moveVec); //캐릭터 방향
            if (moveVec != Vector3.zero) //이동 체크
            {
                isMove = true;
            }
            else
            {
                isMove = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isAttack = true;
            }
        }

        
    }
}
