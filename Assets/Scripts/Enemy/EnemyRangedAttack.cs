using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private PlayerManager _playerManager;
    private EnemyData _data;
    private GameObject _projectilePrefab;//원거리 투사체
    private float _projectileSpeed;
    private TrailRenderer _trailRenderer;
    private Animator _animator;
    private float _attackStartTime;
    private float _attackEndTime;
    private bool _isShoot;
    private float _damage;
    public float Damage => _damage;
    public float ProjectileSpeed => _projectileSpeed;
    //적 캐릭터 패턴이 다양해 지면 적용 어려워짐...추상화 리팩터링 필요
    private void Awake()
    {
        _enemyManager = GetComponentInParent<EnemyManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _data = _enemyManager.Data;
        _attackStartTime = _data.AttackStartFrame / _data.AttackFullFrame;
        _attackEndTime = _data.AttackEndFrame / _data.AttackFullFrame;
        _projectilePrefab = _data.ProjectilePrefab;
        _animator = _enemyManager.GetComponent<Animator>();
        _isShoot = false;
        _damage = _data.Damage;
        _projectileSpeed = _data.ProjectileSpeed;
    }
    
    private void Update()
    {
        float animTime = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        
        if (_enemyManager.IsAttack && !_isShoot &&_attackStartTime <= animTime && animTime <= _attackEndTime)
        {
            _isShoot = true;
            GameObject projectile =Instantiate(_projectilePrefab, transform.position, transform.rotation, transform);
        }
        
        if(animTime > _attackEndTime)
            _isShoot = false;
        
    }
    
}
