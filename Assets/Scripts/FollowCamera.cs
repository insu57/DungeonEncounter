using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowCamera : MonoBehaviour //Player Follow Camera(Quarter view) 플레이어 카메라(쿼터뷰) 
{
    
    [SerializeField]
    private Vector3 offset;
    private Transform _target;
    private void Start()
    {
        offset = new Vector3(0, 3, -3);
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    private void Update()
    {
        transform.position = _target.position + offset;
    }
}
