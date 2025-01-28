using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    public int RoomIndex {get; private set;}
    public float RoomWidth {get; private set;}
    public float RoomHeight {get; private set;}
    public Vector3 RoomCenter {get; private set;}
    //public RoomType RoomType => roomType;
    public bool IsCleared {get; private set;}

    [SerializeField] private int enemyMaxNum;
    [SerializeField] private int enemyMinNum;
    [SerializeField] private RoomType roomType;
    [SerializeField] private GameObject floor;
    private BoxCollider _floorBoxCollider;
    private StageManager _stageManager;
    private DoorTriggerCheck[] _doorTriggerChecks;
    private GameObject _chestGameObject;
    public void BlockRoomDoors(bool isBlock)
    {
        //Room 하위(자식 오브젝트)의 Door Block 
        foreach (var door in _doorTriggerChecks)
        {
            door.BlockDoor(isBlock);
        }
    }

    public void InitRoom()
    {
        switch (roomType)
        {
            case RoomType.NormalRoom:
            case RoomType.BossRoom:
                IsCleared = false;
                break;
            case RoomType.ChestRoom:
                IsCleared = true;
                if (_chestGameObject && _chestGameObject.activeSelf)
                {
                    ObjectPoolingManager.Instance.ReturnToPool(PoolKeys.Chest01, _chestGameObject);
                }
                var chest = ObjectPoolingManager.Instance.GetObjectFromPool(PoolKeys.Chest01, RoomCenter, Quaternion.identity)
                    .GetComponent<Chest>();
                var randomWeaponWeight = Random.value;
                //RandomItem
                chest.SetItem(randomWeaponWeight < 0.5 ? 
                    _stageManager.GetRandomWeaponData().GetItemPrefab()
                    : _stageManager.GetRandomEquipmentData().GetItemPrefab());
                break;
            case RoomType.NpcRoom:
            case RoomType.StartRoom:
            case RoomType.EndRoom:
                IsCleared = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(RoomType) + " Missing");
        }
        BlockRoomDoors(false);
    }
    
    public void RoomCleared()
    {
        //방 클리어 시
        IsCleared = true;
        BlockRoomDoors(false);
    }

    public int GetEnemyNumber()
    {
        return Random.Range(enemyMinNum, enemyMaxNum + 1);
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }
    
    private void Awake()
    {
        _stageManager = FindObjectOfType<StageManager>();
        _floorBoxCollider = floor.GetComponent<BoxCollider>();
        
        RoomIndex = StageManager.GetRoomIndex(transform);
        RoomWidth = _floorBoxCollider.size.x;
        RoomHeight = _floorBoxCollider.size.y;
        RoomCenter = _floorBoxCollider.bounds.center;//bounds.center -> World
        _doorTriggerChecks = GetComponentsInChildren<DoorTriggerCheck>();//하위 Doors
        
    }

    private void Start()
    {
        InitRoom();
        _stageManager.Rooms.Add(this);//StageManager Rooms에 추가
    }
    
}
