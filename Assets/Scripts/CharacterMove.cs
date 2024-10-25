using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [SerializeField]
    float charSpeed = 5.0f;
    Vector3 charDirection;
    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(charDirection*charSpeed*Time.deltaTime);
    }
    
    public void MoveTo(Vector3 direction)
    {
        charDirection = direction;
    }
    public void ChangeSpeed(float speed)
    {
        charSpeed = speed; 
    }
}
