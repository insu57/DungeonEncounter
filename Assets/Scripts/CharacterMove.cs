using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMove : MonoBehaviour //상속?? 일단 남기기 Player 내부에 구현... 
{
    [SerializeField] private float charSpeed = 5.0f;
    private Vector3 charDirection;
    private CharacterController characterController;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        characterController.Move(charDirection * (charSpeed * Time.deltaTime));
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
