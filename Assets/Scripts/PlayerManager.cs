using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    CharacterMove charMove;
    float hAxis; //x��
    float vAxis; //z��
    // Start is called before the first frame update
    void Start()
    {
        charMove = GetComponent<CharacterMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //x,z�� ���� �̵�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        charMove.MoveTo(new Vector3(hAxis,0,vAxis).normalized);
    }
}
