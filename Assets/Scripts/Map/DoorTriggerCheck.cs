using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerCheck : MonoBehaviour
{
    private StageManager _stageManager;
    private RoomManager _roomManager;
    private int _roomIndex = -1;
    [SerializeField] private GameObject blockObject;
    private BoxCollider _doorTrigger;
    public void BlockDoor(bool isBlock)
    {
        blockObject.SetActive(isBlock);
        _doorTrigger.enabled = !isBlock;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var playerIdx = StageManager.GetRoomIndex(other.transform);
        if (playerIdx != _roomIndex) return;
        Debug.Log("Player Enter Room: " + _roomIndex.ToString("D2"));
        var isCleared =  _stageManager.CheckIsCleared(_roomIndex);
        _roomManager.BlockRoomDoors(!isCleared);//클리어 상태가 아니면 Block
        if (isCleared) return;
        _stageManager.SpawnEnemy(_roomManager);
    }

    private void Awake()
    {
        _stageManager = FindObjectOfType<StageManager>();
        _roomManager = GetComponentInParent<RoomManager>();
        _doorTrigger = GetComponent<BoxCollider>();//Door의 Trigger
    }

    private void Start()
    {
        _roomIndex = _roomManager.RoomIndex;
    }
}
