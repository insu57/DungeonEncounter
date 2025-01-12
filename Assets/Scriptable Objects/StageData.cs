using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PoolingObject
{
    public PoolKeys poolKey;
    public GameObject prefab;
    public int defaultCapacity;
    public int maxSize;
}

[CreateAssetMenu(fileName = "StageData", 
    menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [SerializeField] private string stageName;
    [SerializeField] private int roomNumber;
    [SerializeField] private PoolKeys[] enemyKeys;
    [SerializeField] private PoolKeys[] bossKeys;
    [SerializeField] private List<PoolingObject> poolingObjects;
    
    public string GetStageName(){ return stageName;}
    public int GetRoomNumber() { return roomNumber; }
    public PoolKeys[] GetEnemies() { return enemyKeys; }
    public PoolKeys[] GetBoss() { return bossKeys; }
    public List<PoolingObject> GetPoolingObjects() { return poolingObjects; }
}
