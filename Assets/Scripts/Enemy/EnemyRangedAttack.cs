using Player;
using Scriptable_Objects;
using UnityEngine;

namespace Enemy
{
    public class EnemyRangedAttack : MonoBehaviour
    {
        private EnemyManager _enemyManager;
        private EnemyControl _enemyControl;
        private EnemyData _data;
        private GameObject _projectilePrefab;//원거리 투사체
        private PoolKeys _projectileKey;
        private TrailRenderer _trailRenderer;
        private Animator _animator;
        private float _attackStartTime;
        private float _attackEndTime;
        private bool _isShoot;
        public float Damage { get; private set; }
        public float ProjectileSpeed { get; private set; }
        //적 캐릭터 패턴이 다양해 지면 적용 어려워짐...추상화 리팩터링 필요

        public PoolKeys GetProjectileKey()
        {
            return _projectileKey;
        }
        
        private void Awake()
        {
            _enemyManager = GetComponentInParent<EnemyManager>();
            _enemyControl = GetComponentInParent<EnemyControl>();
            _data = _enemyManager.Data;
            _attackStartTime = _data.AttackStartFrame / _data.AttackFullFrame;
            _attackEndTime = _data.AttackEndFrame / _data.AttackFullFrame;
            _projectilePrefab = _data.ProjectilePrefab;
            _projectileKey = _data.ProjectileKey;
            _animator = _enemyManager.GetComponent<Animator>();
            _isShoot = false;
            Damage = _data.Damage;
            ProjectileSpeed = _data.ProjectileSpeed;
        }
    
        private void Update()
        {
            float animTime = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        
            if (_enemyControl.IsAttack && !_isShoot &&_attackStartTime <= animTime && animTime <= _attackEndTime)
            {
                _isShoot = true;
                //Instantiate(_projectilePrefab, transform.position, transform.rotation, transform);
                GameObject projectileObj = ObjectPoolingManager.Instance.GetObjectFromPool(_projectileKey,transform.position,transform.rotation);//ErrorMessage
                EnemyProjectile projectile = projectileObj.GetComponent<EnemyProjectile>();
                projectile.enabled = true;
                projectile.InitEnemyProjectile(this,_enemyManager.EnemyTargetPos());
            }
        
            if(animTime > _attackEndTime)
                _isShoot = false;
        
        }
    
    }
}
