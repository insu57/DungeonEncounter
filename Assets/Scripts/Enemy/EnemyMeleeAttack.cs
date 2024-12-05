using Player;
using Scriptable_Objects;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttack : MonoBehaviour

    {
        private EnemyManager _enemyManager;
        private EnemyControl _enemyControl;
        private EnemyData _data;
        private float _attackStartTime;
        private float _attackEndTime;
        private Collider _attackArea;
        private TrailRenderer _trailRenderer;
        private Animator _animator;
        private float _damage;
        public float Damage => _damage;
    
        //적 캐릭터 패턴... 리팩터링 필요
        private void Awake()
        {
            _enemyControl = GetComponentInParent<EnemyControl>();
            _enemyManager = GetComponentInParent<EnemyManager>(); 
            _attackArea = GetComponent<Collider>(); //공격 판정 Collider
            _trailRenderer = GetComponentInChildren<TrailRenderer>(); //공격 이펙트
            _animator = _enemyManager.GetComponent<Animator>();
            _data = _enemyManager.Data;
            _attackStartTime = _data.AttackStartFrame / _data.AttackFullFrame;
            _attackEndTime = _data.AttackEndFrame / _data.AttackFullFrame;
            _damage = _data.Damage;
            Debug.Log(_damage);
        }
    
        private void Update()
        {
            float animTime = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);

            if (_enemyControl.IsAttack && _attackStartTime <= animTime && animTime <= _attackEndTime)
            {
                _attackArea.enabled = true;
                _trailRenderer.enabled = true;
            }
            else
            {
                _attackArea.enabled = false;
                _trailRenderer.enabled = false;
            }
       
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Player Hit");
        }
    }
}
