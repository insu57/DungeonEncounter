using System.Collections;
using UnityEngine;

public enum PlayerStates { Idle = 0, Run, Attack, Dodge, UseItem, Damaged ,Global }

public class PlayerManager : MonoBehaviour
{
    private float _maxHealth;
    private float _health;
    private float _maxEnergy;
    private float _energy;
    private bool _isMove;
    private bool _isAttack;
    private bool _isDodge;
    private bool _wasDamaged;
    private bool _isUseItem;
    
    private float _moveSpeed;
    private Vector3 _moveVector;
    private float _hAxis; //x axis x축
    private float _vAxis; //z axis z축
    private Animator _animator;
    private CharacterController _characterController;
    private Camera _mainCamera;
    private Vector3 _lookVector;
    private Quaternion _lookRotation;
    private float _dodgeDuration;
    private float _dodgeDistance;
    private float _dodgeCoolTime;
    private bool _isDodgeCool;

    private GameObject _playerWeapon;
    
    private State<PlayerManager>[] _states;
    private StateMachine<PlayerManager> _stateMachine;

    public PlayerStates CurrentState { private set; get; }
    public Animator PlayerAnimator { private set; get; }
    public float Health { set => _health = Mathf.Max(0, value); get => _health; }
    public float MaxHealth { set => _maxHealth = Mathf.Max(0, value); get => _maxHealth; } 
    public float Energy { set => _energy = Mathf.Max(0, value); get => _energy; }
    public float MaxEnergy { set => _maxEnergy = Mathf.Max(0, value); get => _maxEnergy; }
    public bool IsMove { set => _isMove = value; get => _isMove; }
    public bool IsAttack { set => _isAttack = value; get => _isAttack; }
    public bool IsDodge { set => _isDodge = value; get => _isDodge; }
    
    public void ChangeState(PlayerStates newState)
    {
        CurrentState = newState;
        _stateMachine.ChangeState(_states[(int)newState]);
    }
    public void RevertToPreviousState()
    {
        _stateMachine.RevertToPreviousState();
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        PlayerAnimator = _animator;
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        
        _states = new State<PlayerManager>[7];
        _states[(int)PlayerStates.Idle] = new PlayerAnimState.Idle();
        _states[(int)PlayerStates.Run] = new PlayerAnimState.Run();
        _states[(int)PlayerStates.Attack] = new PlayerAnimState.Attack();
        _states[(int)PlayerStates.Dodge] = new PlayerAnimState.Dodge();
        _states[(int)PlayerStates.UseItem] = new PlayerAnimState.UseItem();
        _states[(int)PlayerStates.Damaged] = new PlayerAnimState.Damaged();
        _states[(int)PlayerStates.Global] =  new PlayerAnimState.StateGlobal();

        _stateMachine = new StateMachine<PlayerManager>();
        _stateMachine.Setup(this, _states[(int)PlayerStates.Idle]); //Beginning Animation Idle 초기 애니메이션 Idle
        _stateMachine.SetGlobalState(_states[(int)PlayerStates.Global]);

        _health = 100f;
        _maxHealth = 100f;
        _energy = 100f;
        _maxEnergy = 100f;
        _isMove = false;
        _isAttack = false;
        _isDodge = false;
        _wasDamaged = false;
        _isUseItem = false;
        _moveSpeed = 5f;
        _dodgeDuration = 0.3f;
        _dodgeDistance = 2.5f;
        _dodgeCoolTime = 0.5f;
        _isDodgeCool = false;
        
        
        _lookRotation = Quaternion.LookRotation(Vector3.back);
        _lookVector = Vector3.back;
    }
    
    private void Update() //구조 수정 필요
    {
        _stateMachine.Execute();
        //x,z axis move
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _moveVector = new Vector3(_hAxis, 0, _vAxis).normalized;
        
        
        
        if (!_isAttack && !_isDodge) //Take Action...movement restriction 공격 등 행동 시 이동 제한 
        {
            _isMove = _moveVector != Vector3.zero; //Movement Check 이동 체크
            
            if (!_isDodgeCool)
            {
                if (Input.GetKeyDown(KeyCode.Space)) //회피
                {
                    Vector3 dodgeTarget = transform.position + _lookVector * _dodgeDistance;
                    StopCoroutine(Dodge(dodgeTarget));
                    StartCoroutine(Dodge(dodgeTarget));
                }
            }
            
            if (_moveVector != Vector3.zero)
            {
                _lookVector = _moveVector;//입력이 없을때 필요한 플레이어 방향 저장
                AudioManager.Instance.PlayFootstep(AudioManager.Footstep.RockFootstep); //Footstep Play
            }
            _lookRotation = Quaternion.LookRotation(_lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, 8f * Time.deltaTime);
            // Slerp 부드러운 회전...8f:회전속도
            
            if (Input.GetMouseButtonDown(0)) //Mouse left click Attack 마우스 좌클릭 공격
            {
                _isAttack = true;
                _moveVector = Vector3.zero; //공격 시 정지
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackSfx); //Sfx Play
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackVoice);
            }
            
            _characterController.Move(_moveVector * (_moveSpeed * Time.deltaTime)); //Player Move 이동
            
            
            /* 원거리 플레이어 회전... 마우스기준 회전
            _lookVec -= transform.position;
            _lookVec.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(_lookVec, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 8f * Time.deltaTime);
            */
            
        }
        
    }
    
    private IEnumerator Dodge(Vector3 dodgeTarget) //회피 계산
    {
        var elapsedTime = 0f;
        _isDodgeCool = true;
        
        while (elapsedTime < _dodgeDuration)
        {
            var newPosition = Vector3.Lerp(transform.position, dodgeTarget, elapsedTime / _dodgeDuration);
            _characterController.Move(newPosition - transform.position);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _characterController.Move(dodgeTarget - transform.position);
        _isDodge = false;
        
        yield return new WaitForSeconds(_dodgeCoolTime);
        _isDodgeCool = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack") && _wasDamaged == false)
        {
            //PlayerWeapon playerWeapon = other.GetComponent<PlayerWeapon>(); //->Enemy
            //_wasDamaged = true;
            //_health -= playerWeapon.Damage;
            //Debug.Log("Enemy Health: " + _health);
            //StartCoroutine(Damaged(0.5f));
        }
    }

    private IEnumerator Damaged(float duration)
    {
        yield return new WaitForSeconds(duration);
        _wasDamaged = false;
    }
}
