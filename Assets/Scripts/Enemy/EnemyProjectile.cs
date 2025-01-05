using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        private PoolKeys _projectileKey;
        private Vector3 _targetPos;
        private Vector3 _startPos;
        private Vector3 _direction;
        private Rigidbody _rigidbody;
        private TrailRenderer _trailRenderer;
        private float _speed;
        private EnemyRangedAttack _enemyRangedAttack;

        public float Damage { get; private set; }

        public void InitEnemyProjectile(EnemyRangedAttack enemyRangedAttack, Vector3 targetPos)
        {
            _enemyRangedAttack = enemyRangedAttack;
            _projectileKey = _enemyRangedAttack.GetProjectileKey();
            _speed = _enemyRangedAttack.ProjectileSpeed;
            Damage = _enemyRangedAttack.Damage; //Data받아오기
            _targetPos = targetPos;
            _targetPos.y += 1f;
            _startPos = _enemyRangedAttack.transform.position;
            _direction = (_targetPos - _startPos).normalized;
            transform.rotation = Quaternion.LookRotation(_direction);
            
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            //속력 초기화
            
            //TrailRenderer Init
            _trailRenderer.Clear();
            
            StartCoroutine(ReturnEnemyProjectile());
        }

        private IEnumerator ReturnEnemyProjectile()//2초 지나면 비활성화
        {
            yield return new WaitForSeconds(2f);
            ObjectPoolingManager.Instance.ReturnToPool(_projectileKey, gameObject);
            //null check할 때 Debug.Log때문(실질적으로 비용이 높지않음)
        }

    private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }
    
        private void FixedUpdate()
        {
            _rigidbody.AddForce(_direction * _speed, ForceMode.Impulse);//화살이동
        }

        private void Update()
        {
            if (_rigidbody.velocity != Vector3.zero)//각도 조절
            {
                Quaternion targetRotation = Quaternion.LookRotation(_rigidbody.velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Floor"))//Player or Floor 충돌시
            {
                ObjectPoolingManager.Instance.ReturnToPool(_projectileKey,gameObject);//비활성
            }
        }
    }
}
