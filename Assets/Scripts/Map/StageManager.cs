using System;
using System.Numerics;
using System.Collections.Generic;
using Enemy;
using Player;
using Scriptable_Objects;
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
    private int _enemyNumber = 0;
    private int _enemyKillCount = 0;
   
    private Chest[] _chests;
    
    public void ResetStage()
    {
        foreach (var room in Rooms)
        {
            room.RoomReset();
        }
        var enemies = FindObjectsOfType<EnemyManager>();
        foreach (var enemy in enemies)
        {
            enemy.EnemyOnStageReset();
        }
    }
    
    public void SpawnEnemy(RoomManager room)
    {
        _currentRoom = room;
        //Spawn
        float halfW = room.RoomWidth * 0.5f;
        float halfH = room.RoomHeight * 0.5f;
        Vector3 center = new Vector3(room.RoomCenter.x,0,room.RoomCenter.z); //중심 Position
        _enemyNumber = room.GetEnemyNumber(); //Room의 적의 수 정보
        int areaMask = 1 << room.RoomIndex+3;
        
        const float playerMinRadius = 4f; //플레이어 4f이상 떨어져야함
        const int retryMaxCount = 100; //무한루프 방지 랜덤위치 재시도 제한
        int retryCount = 0;
        for (int i = 0; i < _enemyNumber; i++)
        {
            float distance = 0f;
            Vector3 randomPos;
            do
            {
                //Debug.Log("Retry:" +retryCount);
                randomPos = new Vector3(Random.Range(-halfW, halfW), 0f,
                    Random.Range(-halfH, halfH)) + room.RoomCenter; //무작위 위치
                distance = Vector3.Distance(randomPos, _player.transform.position); //무작위 위치와 플레이어 거리
                retryCount++;
            } while (distance <= playerMinRadius && retryCount <= retryMaxCount);
            
            PoolKeys keys = _enemyKeys[Random.Range(0, _enemyKeys.Length)];
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2f, areaMask))
            {
                //Debug.Log("RandomPos_hit_pos: " + hit.position);
                GameObject enemy = ObjectPoolingManager.Instance
                    .GetObjectFromPool(keys, randomPos, Quaternion.identity);
                
                EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
                // Debug.Log("EnemySpawn: "+randomPos);
                enemyManager.InitEnemySpawn(randomPos);
                enemyManager.OnDeath += HandleEnemyDeath;
            }
        }
    }

    public void HandleEnemyDeath()
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

    public ConsumableItemData GetRandomConsumableItemData()
    {
        var randomIdx = Random.Range(0, stageData.GetConsumableItems().Length);   
        return stageData.GetConsumableItems()[randomIdx];
    }

    public PlayerWeaponData GetRandomWeaponData()
    {
        var randomIdx = Random.Range(0, stageData.GetPlayerWeapons().Length);
        return stageData.GetPlayerWeapons()[randomIdx];
    }

    public PlayerEquipmentData GetRandomEquipmentData()
    {
        var randomIdx = Random.Range(0, stageData.GetPlayerEquipments().Length);
        return stageData.GetPlayerEquipments()[randomIdx];
    }
    
    private void Awake()
    {
        _roomNumber = stageData.GetRoomNumber(); //StageData 받아오기
        _enemyKeys = stageData.GetEnemies();
        ObjectPoolingManager.Instance.InitStagePools(stageData.GetPoolingObjects());
        _chests = FindObjectsOfType<Chest>();
        foreach (var chest in _chests)
        {
            var randomWeaponWeight = Random.value;
            if (randomWeaponWeight < 0.5)
            {
                //weapon
                var randomWeaponIdx = Random.Range(0, stageData.GetPlayerWeapons().Length);
                PlayerWeaponData weaponData = stageData.GetPlayerWeapons()[randomWeaponIdx];
                chest.SetItem(weaponData.GetItemPrefab());
            }
            else
            {
                //equipment
                var randomEquipmentIdx = Random.Range(0, stageData.GetPlayerEquipments().Length);
                PlayerEquipmentData equipmentData = stageData.GetPlayerEquipments()[randomEquipmentIdx];
                chest.SetItem(equipmentData.GetItemPrefab());
            }
        }
    }
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        PlayerRoomIdx = 0; //초기 Room00
    }
    
}
