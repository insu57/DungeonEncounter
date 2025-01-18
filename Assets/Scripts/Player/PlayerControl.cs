using System;
using System.Collections;
using UnityEngine;

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
        
        private bool _isAttackCool;//공격 등 입력 쿨타임
        private float _isAttackCoolTime;
        
        private State<PlayerControl>[] _states;
        private StateMachine<PlayerControl> _stateMachine;
        
        public PlayerStates CurrentState { private set; get; }
        public Animator PlayerAnimator { private set; get; }
        private string _currentAnimation = "";
        public bool IsMove { private set; get; }
        public bool IsAttack { set; get; }
        public bool IsSkill { set; get; }
        public bool IsDodge { private set; get; }
        public bool IsDamaged { set; get; }
        
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
                IsDodge = true;
                var newPosition = Vector3.Lerp(transform.position, dodgeTarget, elapsedTime / _dodgeDuration);
                _characterController.Move(newPosition - transform.position);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _characterController.Move(dodgeTarget - transform.position);
            IsDodge = false;
        
            yield return new WaitForSeconds(_dodgeCoolTime); //회피 쿨타임
            _isDodgeCool = false;
        }

        private IEnumerator AttackCool()
        {
            _isAttackCool = true;
            yield return new WaitForSeconds(_isAttackCoolTime);
            _isAttackCool = false;
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
            
            IsMove = false;
            IsAttack = false;
            IsSkill = false;
            IsDodge = false;
            IsDamaged = false;
            
            _moveSpeed = 7f; //5f SO에서 가져오게 수정
            _dodgeDuration = 0.3f;
            _dodgeDistance = 2.5f;
            _dodgeCoolTime = 0.5f;
            _isDodgeCool = false;
            
            _isAttackCool = false;
            _isAttackCoolTime = 0.2f;
        
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
            
            _playerManager.ActiveSwordAttackBox(IsAttack, IsSkill);

            if (Input.GetKeyDown(KeyCode.F))
            {
                _playerManager.GetItemInRange();
            }
            
            if (!IsAttack && !IsDodge) //Take Action...movement restriction 공격 등 행동 시 이동 제한 
            {
                IsMove = _moveVector != Vector3.zero; //Movement Check 이동 체크
            
                if (!_isDodgeCool)
                {
                    if (Input.GetKeyDown(KeyCode.Space)) //회피
                    {
                        Vector3 dodgeTarget = transform.position + _lookVector * _dodgeDistance;
                        StopCoroutine(Dodge(dodgeTarget));
                        StartCoroutine(Dodge(dodgeTarget));
                    }
                }

                if (Input.GetMouseButtonDown(0) && !_isAttackCool) //Mouse left click Attack 마우스 좌클릭 공격
                {
                    //StartCoroutine(AttackCool());
                    _moveVector = Vector3.zero; //공격 시 정지
                    IsAttack = true;

                    AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackSfx); //Sfx Play
                    AudioManager.Instance.PlaySfx(AudioManager.Sfx.AttackVoice);
                    //????

                }

                if (Input.GetMouseButtonDown(1) && _playerManager.GetStat(PlayerStatTypes.Energy) >= 100f
                                                && !_isAttackCool)
                    //우클릭 스킬 공격, 에너지 100이상일때만
                {
                    _moveVector = Vector3.zero;
                    IsAttack = true;
                    IsSkill = true;
                    _playerManager.UseSkill();//플레이어 스탯 에너지 소모
                    //sound
                }
                
            
                if (_moveVector != Vector3.zero)
                {
                    _lookVector = _moveVector;//입력이 없을때 필요한 플레이어 방향 저장
                    AudioManager.Instance.PlayFootstep(AudioManager.Footstep.RockFootstep); //Footstep Play
                }
                _lookRotation = Quaternion.LookRotation(_lookVector);
                transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 8f * Time.deltaTime);
                // lerp 부드러운 회전...8f:회전속도
                
                _characterController.Move(_moveVector * (_moveSpeed * Time.deltaTime)); //Player Move 이동

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
            
                /* 원거리 플레이어 회전... 마우스기준 회전
            _lookVec -= transform.position;
            _lookVec.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(_lookVec, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 8f * Time.deltaTime);
            */
            
            }
            
            
        }
    }
}
