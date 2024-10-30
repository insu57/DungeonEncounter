using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem, Global }

public class PlayerManager : MonoBehaviour 
{
    private int health;
    private int energy;
    private int dodgeGauge;
    private bool isMove;
    private bool isAttack;

    private Vector3 moveVec;
    private CharacterMove charMove;
    private float hAxis; //x axis x축
    private float vAxis; //z axis z축
    private Animator animator;

    private State<PlayerManager>[] states;
    private StateMachine<PlayerManager> stateMachine;

    public PlayerStates CurrentState { private set; get; }
    public Animator PlayerAnimator { private set; get; }

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

    public  bool IsMove { get; set; }
    public bool IsAttack { get; set; }

    public void ChangeState(PlayerStates newState)
    {
        CurrentState = newState;
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
        PlayerAnimator = animator;

        states = new State<PlayerManager>[6];
        states[(int)PlayerStates.Idle] = new PlayerAnimState.Idle();
        states[(int)PlayerStates.Run] = new PlayerAnimState.Run();
        states[(int)PlayerStates.Attack] = new PlayerAnimState.Attack();
        states[(int)PlayerStates.Dodge] = new PlayerAnimState.Dodge();
        states[(int)PlayerStates.UseItem] = new PlayerAnimState.UseItem();
        states[(int)PlayerStates.Global] =  new PlayerAnimState.StateGlobal();

        stateMachine = new StateMachine<PlayerManager>();
        stateMachine.Setup(this, states[(int)PlayerStates.Idle]); //Beginning Animation Idle 초기 애니메이션 Idle
        stateMachine.SetGlobalState(states[(int)PlayerStates.Global]);

        health = 100;
        energy = 100;
        dodgeGauge = 100;
        isMove = false;
        isAttack = false;
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Execute();

        //x,z axis move
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (!isAttack) //Take Action(such as Attack) movement restriction 공격 등 행동 시 이동 제한 
        {
            if (Input.GetMouseButtonDown(0)) //Mouse left click Attack 마우스 좌클릭 공격
            {
                isAttack = true;
                moveVec = Vector3.zero;
            }

            charMove.MoveTo(moveVec); //WASD KeyInput Move 키입력 기준 이동
            transform.LookAt(transform.position + moveVec); //Player Direction 플레이어 방향

            isMove = moveVec != Vector3.zero; //Movement Check 이동 체크
        }
    }
}
