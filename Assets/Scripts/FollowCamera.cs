using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour //Player Follow Camera(Quarter view) 플레이어 카메라(쿼터뷰) 
{
   
    public Transform target;
    [SerializeField]
    private Vector3 offset;
    void Start()
    {
        offset = new Vector3(0, 3, -3);
    }
    
    void Update()
    {
        transform.position = target.position + offset;
    }
}
