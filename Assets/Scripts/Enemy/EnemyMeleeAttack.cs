using Player;
using Scriptable_Objects;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttack : MonoBehaviour

    {
        private EnemyManager _enemyManager;
        protected EnemyControl EnemyControl;
        private EnemyData _data;
        protected float AttackStartTime;
        protected float AttackEndTime;
        [SerializeField] protected Collider attackArea;
        [SerializeField] protected TrailRenderer trailRenderer;
        //private TrailRenderer _trailRenderer;
        protected Animator Animator;
        private float _damage;
        public float Damage => _damage;
    
        //적 캐릭터 패턴... 리팩터링 필요
        protected virtual void Awake()
        {
            EnemyControl = GetComponentInParent<EnemyControl>();
            _enemyManager = GetComponentInParent<EnemyManager>(); 
            Animator = _enemyManager.GetComponent<Animator>();
            _data = _enemyManager.Data;
            AttackStartTime = _data.AttackStartFrame / _data.AttackFullFrame;
            AttackEndTime = _data.AttackEndFrame / _data.AttackFullFrame;
            _damage = _data.Damage;
        }

        protected virtual void Update()
        {
            float animTime = Mathf.Repeat(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);

            if (EnemyControl.IsAttack && AttackStartTime <= animTime && animTime <= AttackEndTime)
            {
                attackArea.enabled = true;
                trailRenderer.enabled = true;
            }
            else
            {
                attackArea.enabled = false;
                trailRenderer.Clear();
                trailRenderer.enabled = false;
            }
       
        }

    }
}
