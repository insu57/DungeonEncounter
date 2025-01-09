using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StageData", 
    menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [SerializeField] private string stageName;
    [SerializeField] private int roomNumber;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] boss;
    
    public string GetStageName(){ return stageName;}
    public int GetRoomNumber() { return roomNumber; }
    public GameObject[] GetEnemies() { return enemies; }
    public GameObject[] GetBoss() { return boss; }
}
