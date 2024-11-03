using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem, Global }

public class PlayerManager : MonoBehaviour
{
    private int health;
    private int energy;
    private int dodgeGauge;
    private bool _isMove;
    private bool _isAttack;
    private bool _isDodge;
    
    private float moveSpeed;
    private Vector3 moveVector;
    private float hAxis; //x axis x축
    private float vAxis; //z axis z축
    private Animator _animator;
    private CharacterController _characterController;
    private Camera _mainCamera;
    private Vector3 _lookVector;
    private Quaternion _lookRotation;
    private float _dodgeDuration = 2f;
    
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

    public bool IsMove
    {
        set => _isMove = value; 
        get => _isMove;
    }

    public bool IsAttack
    {
        set => _isAttack = value;
        get => _isAttack;
    }

    public bool IsDodge
    {
        set => _isDodge = value;
        get => _isDodge;
    }
    
    public void ChangeState(PlayerStates newState)
    {
        CurrentState = newState;
        stateMachine.ChangeState(states[(int)newState]);
    }
    public void RevertToPreviousState()
    {
        stateMachine.RevertToPreviousState();
    }


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        PlayerAnimator = _animator;
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        
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
        _isMove = false;
        _isAttack = false;
        _isDodge = false;
        moveSpeed = 5f;
        
        _lookRotation = Quaternion.LookRotation(Vector3.back);
        _lookVector = Vector3.back;
    }

    // Update is called once per frame
    private void Update()
    {
        stateMachine.Execute();
        //x,z axis move
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVector = new Vector3(hAxis, 0, vAxis).normalized;
        if (moveVector != Vector3.zero)
        {
            _lookVector = moveVector;//입력이 없을때 필요한 플레이어 방향 저장
            AudioManager.Instance.PlayFootstep(AudioManager.Footstep.RockFootstep); //Footstep Play
        }
        _lookRotation = Quaternion.LookRotation(_lookVector);
        
        if (!_isAttack && !_isDodge) //Take Action...movement restriction 공격 등 행동 시 이동 제한 
        {
            //transform.LookAt(transform.position + moveVec); //Player Direction 플레이어 방향
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 8f * Time.deltaTime);
            // Slerp 부드러운 회전...8f:회전속도
            
            if (Input.GetMouseButtonDown(0)) //Mouse left click Attack 마우스 좌클릭 공격
            {
                _isAttack = true;
                moveVector = Vector3.zero; //공격 시 정지
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackSfx); //Sfx Play
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackVoice);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _characterController.Move(_lookVector * (30f * Time.deltaTime));
                //선형보간Lerp? 코루틴? 1~3초??
                
            }
            
            
            _characterController.Move(moveVector * (moveSpeed * Time.deltaTime)); //Player Move 이동
            
            /* 원거리 플레이어 회전... 마우스기준 회전
            _lookVec -= transform.position;
            _lookVec.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(_lookVec, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 8f * Time.deltaTime);
            */
            _isMove = moveVector != Vector3.zero; //Movement Check 이동 체크
        }
        
    }

    IEnumerable DodgePosition() //회피 코루틴
    {

        yield return null;
    }
    
}
