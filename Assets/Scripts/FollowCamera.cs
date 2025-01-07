
using Player;
using UnityEngine;


public class FollowCamera : MonoBehaviour //Player Follow Camera(Quarter view) 플레이어 카메라(쿼터뷰) 
{
    
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Vector3 rotation;
    private Transform _target;
    private void Start()
    {
        offset = new Vector3(0, 4, -4);
        rotation = new Vector3(35, 0, 0);
        _target = FindObjectOfType<PlayerManager>().transform;
        transform.rotation = Quaternion.Euler(rotation);
    }
    
    private void Update()
    {
        transform.position = _target.position + offset;
    }
}
