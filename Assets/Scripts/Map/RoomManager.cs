using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

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
    
    public void BlockRoomDoors(bool isBlock)
    {
        //Room 하위(자식 오브젝트)의 Door Block 
        foreach (var door in _doorTriggerChecks)
        {
            door.BlockDoor(isBlock);
        }
    }

    public void RoomReset()
    {
        switch (roomType)
        {
            case RoomType.NormalRoom:
            case RoomType.BossRoom:
                IsCleared = false;
                break;
            case RoomType.ChestRoom:
                break;
            case RoomType.NpcRoom:
                break;
            default:
                break;
        }
        BlockRoomDoors(false);
        var enemies = FindObjectsOfType<EnemyManager>();
        foreach (var enemy in enemies)
        {
            ObjectPoolingManager.Instance.ReturnToPool(enemy.key, enemy.gameObject);
        }
        //ObjectPoolingManager.Instance.
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
    
    private void Awake()
    {
        _stageManager = FindObjectOfType<StageManager>();
        _floorBoxCollider = floor.GetComponent<BoxCollider>();
        
        RoomIndex = StageManager.GetRoomIndex(transform);
        RoomWidth = _floorBoxCollider.size.x;
        RoomHeight = _floorBoxCollider.size.y;
        RoomCenter = _floorBoxCollider.bounds.center;//bounds.center -> World
        IsCleared = roomType is not (RoomType.BossRoom or RoomType.NormalRoom);
        
        _doorTriggerChecks = GetComponentsInChildren<DoorTriggerCheck>();//하위 Doors
    }

    private void Start()
    {
        _stageManager.Rooms.Add(this);//StageManager Rooms에 추가
    }
    
}
