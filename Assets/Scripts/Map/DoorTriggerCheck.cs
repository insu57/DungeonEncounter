using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerCheck : StageManager
{
    private int _roomIndex = -1;
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int playerIdx = GetRoomIndex(other.transform);
            if (playerIdx == _roomIndex)
            {
                PlayerRoomIdx = playerIdx;
                Debug.Log("Player Enter Room: " + _roomIndex.ToString("D2"));
            }
        }
    }

    public override void Start()
    {
        
    }
    
    private void Awake()
    {
        _roomIndex = GetRoomIndex(transform);
    }

    public override void Update()
    {
        
    }
}
