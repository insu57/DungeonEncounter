using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private Collider _attackArea;
    private TrailRenderer _trailRenderer;
    private float _damage;
    public float Damage => _damage;
    
    private void Awake()
    {
        _enemyManager = GetComponentInParent<EnemyManager>();
        _attackArea = GetComponent<Collider>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
      
    }

    private void Start()
    {
        _damage = _enemyManager.Power;
    }

    private void Update()
    {
        if (_enemyManager.IsAttack)
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
