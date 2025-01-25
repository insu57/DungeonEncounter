using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
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
    [SerializeField] private EnemyData[] bossData;
    [SerializeField] private List<PoolingObject> poolingObjects;
    [SerializeField] private PlayerWeaponData[] playerWeapons;
    [SerializeField] private PlayerEquipmentData[] playerEquipments;
    [SerializeField] private ConsumableItemData[] consumableItems;
    public string GetStageName(){ return stageName;}
    public int GetRoomNumber() { return roomNumber; }
    public PoolKeys[] GetEnemies() { return enemyKeys; }
    public EnemyData[] GetBoss() { return bossData; }
    public PlayerWeaponData[] GetPlayerWeapons() { return playerWeapons; }
    public PlayerEquipmentData[] GetPlayerEquipments() { return playerEquipments; }
    public ConsumableItemData[] GetConsumableItems() { return consumableItems; }
    public List<PoolingObject> GetPoolingObjects() { return poolingObjects; }
}
