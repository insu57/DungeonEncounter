using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem }

public class PlayerManager : MonoBehaviour //진행중 상태머신?????? 애니메이션->StateMachine 플레이어 Move 분리????
{
    private int health;
    private int energy;
    private int dodgeGauge;

    private CharacterMove charMove;
    private float hAxis; //x축
    private float vAxis; //z축
    private Vector3 moveVec;
    public Animator animator;

    private State<PlayerManager>[] states;
    private StateMachine<PlayerManager> stateMachine;

    /*
     
     public void Setup()
    {
        states = new State<PlayerManager>[5];
        states[(int)PlayerStates.Idle] = new PlayerAnimState.Idle();
        states[(int)PlayerStates.Run] = new PlayerAnimState.Run();
        states[(int)PlayerStates.Attack] = new PlayerAnimState.Attack();
        states[(int)PlayerStates.Dodge] = new PlayerAnimState.Dodge();
        states[(int)PlayerStates.UseItem] = new PlayerAnimState.UseItem();

        health = 100;
        energy = 100;
        dodgeGauge = 100;
    } 

    public void Updated()
    {
        stateMachine.Execute();
    }
     */
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

    public void ChangeState(PlayerStates newState)
    {
        stateMachine.ChangeState(states[(int)newState]);
    }


        void Awake()
    {
        charMove = GetComponent<CharacterMove>();
        animator = GetComponentInChildren<Animator>();

        states = new State<PlayerManager>[5];
        states[(int)PlayerStates.Idle] = new PlayerAnimState.Idle();
        states[(int)PlayerStates.Run] = new PlayerAnimState.Run();
        states[(int)PlayerStates.Attack] = new PlayerAnimState.Attack();
        states[(int)PlayerStates.Dodge] = new PlayerAnimState.Dodge();
        states[(int)PlayerStates.UseItem] = new PlayerAnimState.UseItem();

        health = 100;
        energy = 100;
        dodgeGauge = 100;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Execute(); 

        //x,z축 방향 이동
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        charMove.MoveTo(moveVec);


        animator.SetBool("isRun", moveVec != Vector3.zero);
    }
}
