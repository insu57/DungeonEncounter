using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _targetPos;
    private Vector3 startPos;
    private Rigidbody _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _targetPos = FindObjectOfType<PlayerManager>().transform.position;
        Debug.Log(_targetPos);
    }

    private void Update()
    {
        _rigidbody.AddForce(_targetPos * (Time.deltaTime * 10f));
    }
}
