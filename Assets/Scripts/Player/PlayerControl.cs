using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public enum PlayerStates
    {
        Idle = 0, Run, Attack, Dodge, Global
    }
    public class PlayerControl : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private Animator _animator;
        private CharacterController _characterController;
        private float _moveSpeed;
        private Vector3 _moveVector;
        private float _hAxis; //x axis x축
        private float _vAxis; //z axis z축
        private Vector3 _lookVector;
        private Quaternion _lookRotation;
        private float _dodgeDuration;
        private float _dodgeDistance;
        private float _dodgeCoolTime;
        private bool _isDodgeCool;
        
        private State<PlayerControl>[] _states;
        private StateMachine<PlayerControl> _stateMachine;
        
        public PlayerStates CurrentState { private set; get; }
        public Animator PlayerAnimator { private set; get; }
        private string _currentAnimation = "";
  
        public bool IsSkill { set; get; }

        private InventoryUIView _inventoryUIView; //UI presenter 수정필요
        
        public void ChangeState(PlayerStates newState)
        {
            CurrentState = newState;
            _stateMachine.ChangeState(_states[(int)newState]);
        }
        public void ChangeAnimation(string newAnimation, float crossFadeTime = 0.2f)
        {
            //애니메이션 변경
            if (_currentAnimation != newAnimation)
            {
                
                if (newAnimation == "Attack")
                {
                    PlayerAnimator.Play("Attack",0,0f);
                }
                else if (newAnimation == "Skill")
                {
                    PlayerAnimator.Play("Skill");
                }
                else
                {
                    //crossFade전환문제
                    //PlayerAnimator.Play(newAnimation,0,0f);
                    PlayerAnimator.CrossFade(newAnimation, crossFadeTime, -1, 0);
                }
                _currentAnimation = newAnimation;
            }
        }
        private IEnumerator Dodge(Vector3 dodgeTarget) //회피 계산
        {
            var elapsedTime = 0f;
            _isDodgeCool = true;
        
            while (elapsedTime < _dodgeDuration) //dodgeDuration동안 dodgeTarget으로 움직임(lerp)
            {
                //IsDodge = true;
                ChangeState(PlayerStates.Dodge);
                var newPosition = Vector3.Lerp(transform.position, dodgeTarget, elapsedTime / _dodgeDuration);
                _characterController.Move(newPosition - transform.position);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _characterController.Move(dodgeTarget - transform.position);
            //IsDodge = false;
            ChangeState(PlayerStates.Idle);
        
            yield return new WaitForSeconds(_dodgeCoolTime); //회피 쿨타임
            _isDodgeCool = false;
        }
        
        private void Awake()
        {
            _playerManager = GetComponent<PlayerManager>();
            _animator = GetComponent<Animator>();
            PlayerAnimator = _animator;
            _characterController = GetComponent<CharacterController>();
            
            _states = new State<PlayerControl>[5];
            _states[(int)PlayerStates.Idle] = new Idle();
            _states[(int)PlayerStates.Run] = new Run();
            _states[(int)PlayerStates.Attack] = new Attack();
            _states[(int)PlayerStates.Dodge] = new Dodge();
            _states[(int)PlayerStates.Global] =  new StateGlobal();
            
            _stateMachine = new StateMachine<PlayerControl>();
            _stateMachine.Setup(this, _states[(int)PlayerStates.Idle]); //Beginning Animation Idle 초기 애니메이션 Idle
            _stateMachine.SetGlobalState(_states[(int)PlayerStates.Global]);
            
            IsSkill = false; ;
            
            _moveSpeed = 7f; //5f SO에서 가져오게 수정
            _dodgeDuration = 0.3f;
            _dodgeDistance = 2.5f;
            _dodgeCoolTime = 0.5f;
            _isDodgeCool = false;
        
            _lookRotation = Quaternion.LookRotation(Vector3.back);
            _lookVector = Vector3.back;
            
        }

        private void Update()
        {
            if(GameManager.Instance.GamePaused) return;
        
            _stateMachine.Execute();
            
            //x,z axis move
            _hAxis = Input.GetAxisRaw("Horizontal");
            _vAxis = Input.GetAxisRaw("Vertical");
            _moveVector = new Vector3(_hAxis, 0, _vAxis).normalized;

            var IsAttack = CurrentState == PlayerStates.Attack;
            var IsDodge = CurrentState == PlayerStates.Dodge;
            _playerManager.ActiveSwordAttackBox(IsAttack, IsSkill);
            _playerManager.ActiveDodgeEffect(IsDodge);
            
            //FSM 수정
            if (CurrentState != PlayerStates.Attack && CurrentState != PlayerStates.Dodge)
            {
                if (_moveVector != Vector3.zero)
                {
                    ChangeState(PlayerStates.Run);
                    _lookVector = _moveVector;//입력이 없을때 필요한 플레이어 방향 저장
                    AudioManager.Instance.PlayFootstep(AudioManager.Footstep.RockFootstep); //Footstep Play
                
                    _lookRotation = Quaternion.LookRotation(_lookVector);
                    transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 8f * Time.deltaTime);
                    // lerp 부드러운 회전...8f:회전속도
                
                    _characterController.Move(_moveVector * (_moveSpeed * Time.deltaTime)); //Player Move 이동
                }
                else
                {
                    ChangeState(PlayerStates.Idle);
                }
            }
            
            if (!EventSystem.current.IsPointerOverGameObject()) //UI 마우스 제한
            {
                Cursor.visible = false;
                if (CurrentState != PlayerStates.Attack)
                {
                    if (Input.GetMouseButtonDown(0)) //Mouse left click Attack 마우스 좌클릭 공격
                    {
                        _moveVector = Vector3.zero; //공격 시 정지
                        //IsAttack = true;
                        ChangeState(PlayerStates.Attack);
                        AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackSfx); //Sfx Play
                    }

                    if (Input.GetMouseButtonDown(1) && _playerManager.GetStat(PlayerStatTypes.Energy) >= 100f)
                        //우클릭 스킬 공격, 에너지 100이상일때만
                    {
                        _moveVector = Vector3.zero;
                        //IsAttack = true;
                        IsSkill = true;
                        ChangeState(PlayerStates.Attack);
                    
                        _playerManager.UseSkill();//플레이어 스탯 에너지 소모
                        //sound
                        AudioManager.Instance.PlaySfx(AudioManager.Sfx.SkillSfx);
                        AudioManager.Instance.PlayVoice(AudioManager.Voice.AttackVoice);
                    }
                }
            }
            else
            {
                Cursor.visible = true;
            }
            
            if (!_isDodgeCool)
            {
                if (Input.GetKeyDown(KeyCode.Space)) //회피
                {
                    Vector3 dodgeTarget = transform.position + _lookVector * _dodgeDistance;
                    AudioManager.Instance.PlaySfx(AudioManager.Sfx.DodgeSfx);
                    StopCoroutine(Dodge(dodgeTarget));
                    StartCoroutine(Dodge(dodgeTarget));
                }
            }
            
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                _playerManager.GetItemInRange();//범위안 아이템 획득 처리
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //QuickSlot1
                _playerManager.UseItemQuickSlot(1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                //QuickSlot2
                _playerManager.UseItemQuickSlot(2);
            }
            
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            {
                _playerManager.OnOpenPlayerMenu?.Invoke();
            }
            
            if (_playerManager.IsPlayerDead)
            {
                _characterController.enabled = false;
                _playerManager.ResetStat();
                _characterController.enabled = true;
            }
            
            /* 원거리 플레이어 회전... 마우스기준 회전
            _lookVec -= transform.position;
            _lookVec.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(_lookVec, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 8f * Time.deltaTime);
            */
        }
    }
}
