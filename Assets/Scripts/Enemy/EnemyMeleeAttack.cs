using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour

{
    private EnemyManager _enemyManager;
    private Collider _attackArea;
    private TrailRenderer _trailRenderer;
    private Animator _animator;
    private float _damage;
    public float Damage => _damage;
    
    private void Awake()
    {
        _enemyManager = GetComponentInParent<EnemyManager>();
        _attackArea = GetComponent<Collider>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _animator = _enemyManager.GetComponent<Animator>();
        
    }

    private void Start()
    {
        _damage = _enemyManager.Damage;
    }

    private void Update()
    {
        float animTime = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f);
        if (_enemyManager.IsAttack && animTime is >= 12f / 64f and <= 24f / 64f) 
            //전체 애니메이션 시간, 공격 판정 시작 시간, 공격 판정 종료 시간 ... 공격 애니메이션 별로 정리(Scriptable object?)
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
}
