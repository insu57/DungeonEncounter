using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    CharacterMove charMove;
    float hAxis; //x축
    float vAxis; //z축
    // Start is called before the first frame update
    void Start()
    {
        charMove = GetComponent<CharacterMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //x,z축 방향 이동
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        charMove.MoveTo(new Vector3(hAxis,0,vAxis).normalized);
    }
}
