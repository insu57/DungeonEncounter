using System;
using System.Numerics;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class StageManager : MonoBehaviour
{
    
    //StageInfo <- SO?
    [SerializeField] private StageData stageData;
    private int _roomNumber;
    private PoolKeys[] _enemyKeys;

    public int PlayerRoomIdx{get; private set;}
    
    private PlayerManager _player;
    private NavMeshHit _hit;

    //private string 
    public readonly List<RoomManager> Rooms = new List<RoomManager>();
    private RoomManager _currentRoom;
    private int _enemyNumber = 5;
    private int _enemyKillCount = 0;
    public void SpawnEnemy(RoomManager room)
    {
        _currentRoom = room;
        //Spawn
        float halfW = room.RoomWidth * 0.5f;
        float halfH = room.RoomHeight * 0.5f;
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-halfW, halfW), 0f,
                Random.Range(-halfH, halfH)) + room.RoomCenter;//무작위 위치
            PoolKeys keys = _enemyKeys[Random.Range(0, _enemyKeys.Length)];
            GameObject enemy = ObjectPoolingManager.Instance.GetObjectFromPool(keys, randomPos, Quaternion.identity);
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            enemyManager.InitEnemySpawn();
            enemyManager.OnDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath()
    {
        _enemyKillCount++;
        if (_enemyKillCount >= _enemyNumber)
        {
            _enemyKillCount = 0;
            _currentRoom.RoomCleared();
        }
    }
    
        
    public static int GetRoomIndex(Transform transform)
    {
        int areaMask;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            areaMask = hit.mask;//transform 기준 area mask
        }
        else
        {
            Debug.LogWarning("No NavMesh Hit Area");
            return -1;
        }
        int roomIndex = -3;
        while (areaMask >> 1 != 0)
        {
            areaMask >>= 1;
            roomIndex++;
        }
        //index값으로 변경(기존 bit형식)
        return roomIndex;
    }

    public bool CheckIsCleared(int roomIndex)
    {
        return Rooms.Find(x => x.RoomIndex == GetRoomIndex(_player.transform))?.IsCleared ?? true;
        //Room의 IsCleared값을 리턴. Rooms에 없으면 True(clear상태)리턴
    }

    private void Awake()
    {
        _roomNumber = stageData.GetRoomNumber(); //StageData 받아오기
        _enemyKeys = stageData.GetEnemies();
    }
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        PlayerRoomIdx = 0; //초기 Room00
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //Clear This Room.
            Rooms.Find(x => x.RoomIndex == GetRoomIndex(_player.transform)).RoomCleared();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            bool isCleared = Rooms.Find(x => x.RoomIndex == GetRoomIndex(_player.transform))?.IsCleared ?? false;
            Debug.Log("IsCleared?: "+ isCleared);
            
        }
    }

    
}
