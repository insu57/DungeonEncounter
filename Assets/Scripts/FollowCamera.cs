using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    [SerializeField]
    private Vector3 offset;
    void Start()
    {
        offset = new Vector3(0, 4, -2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
