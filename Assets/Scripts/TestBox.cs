using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestBox : MonoBehaviour
{
    Box box;
    public GameObject Player;
    Vector3 target = new Vector3(8, 1.5f, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3(0, 0, 1) * -5f*Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, 5f*Time.deltaTime);
    }
}
