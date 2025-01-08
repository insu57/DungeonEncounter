using System.Numerics;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;

public class Stage1Manager : MonoBehaviour
{
    //StageInfo <- SO?

    private PlayerManager _player;

    private NavMeshHit _hit;

    //private string 

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
    
    private void Awake()
    {
        _player = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        
        if (NavMesh.SamplePosition(_player.transform.position, out _hit, 1.0f, NavMesh.AllAreas))
        {
            int areaMask = _hit.mask;
            Debug.Log(RoomCheck(areaMask));
        }
    }
}
