using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomManager : StageManager
{
    private int _roomIndex;
    [SerializeField] private RoomType roomType;

    private void Awake()
    {
        _roomIndex = GetRoomIndex(transform);
        //Debug.Log(gameObject.name+' ' +GetRoomIndex(transform));
    }
    public override void Start()
    {
        Rooms.Add(new Room(_roomIndex,roomType));
    }

    public override void Update()
    {
        
    }
}
