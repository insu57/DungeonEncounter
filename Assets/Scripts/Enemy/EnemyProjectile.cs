using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _targetPos;
    private Vector3 _startPos;
    private Rigidbody _rigidbody;
    private float _speed;
    private EnemyRangedAttack _enemyRangedAttack;
    
    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _targetPos = FindObjectOfType<PlayerManager>().transform.position;
        _targetPos.y += 1f; //player center y
        _startPos = transform.position;
        _enemyRangedAttack = GetComponentInParent<EnemyRangedAttack>();
        _speed = _enemyRangedAttack.ProjectileSpeed;
        
    }

    private void Update()
    {
        _rigidbody.AddForce((_targetPos - _startPos).normalized * _speed ,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }
}
