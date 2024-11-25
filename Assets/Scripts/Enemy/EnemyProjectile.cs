using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _targetPos;
    private Vector3 _startPos;
    private Rigidbody _rigidbody;
    private float _speed;
    private float _damage;
    private EnemyRangedAttack _enemyRangedAttack;
    
    public float Damage => _damage;
    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _targetPos = FindObjectOfType<PlayerManager>().transform.position;
        _targetPos.y += 1f; //player center y
        _startPos = transform.position;
        _enemyRangedAttack = GetComponentInParent<EnemyRangedAttack>();
        _speed = _enemyRangedAttack.ProjectileSpeed;
        _damage = _enemyRangedAttack.Damage;
    }

    private void Start()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        _rigidbody.AddForce((_targetPos - _startPos).normalized * _speed ,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); //추후 오브젝트 풀링으로 관리로 수정 예정
        }
        else if(other.CompareTag("Floor"))
        {
            Destroy(gameObject, 2f);
        }
    }
}
