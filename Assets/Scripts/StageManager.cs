using System.Numerics;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class StageManager : MonoBehaviour, IStageManager
{
    //StageInfo <- SO?
    [SerializeField] private StageData stageData;
    private int _roomNumber;

    protected int PlayerRoomIdx;
    private PlayerManager _player;
    private NavMeshHit _hit;

    //private string 
    
    protected class Room
    {
        public int RoomIndex;
        public readonly RoomType RoomType;
        public bool IsCleared;

        private void RoomSetup()
        {
            IsCleared = RoomType is not (RoomType.BossRoom or RoomType.NormalRoom); //Normal/Boss Room 아니면 Clear상태
        }
        
        public Room(int roomIndex, RoomType roomType)
        {
            RoomIndex = roomIndex;
            RoomType = roomType;
            RoomSetup();
        }
    }

    protected List<Room> Rooms = new List<Room>();
    
    public void SpawnEnemy()
    {
        //Spawn
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
    
    private int RoomCheck(int areaMask)
    {
        int roomIdx = -3; 
        while ((areaMask >> 1) != 0)
        {
            areaMask >>= 1;
            roomIdx ++;
        }

        return roomIdx;
    }

    public virtual void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        PlayerRoomIdx = 0;
        
        _roomNumber = stageData.GetRoomNumber();
        
    }
    
    public virtual void Update()
    {
        /*
        if (NavMesh.SamplePosition(_player.transform.position, out _hit, 1.0f, NavMesh.AllAreas))
        {
            int areaMask = _hit.mask;
            Debug.Log(RoomCheck(areaMask));
        }*/
    }

    
}
