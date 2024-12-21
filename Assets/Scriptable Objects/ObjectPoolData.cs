using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolData", menuName = "ScriptableObjects/ObjectPoolData", order = 1)]
public class ObjectPoolData : ScriptableObject
{
    [SerializeField] GameObject prefab;
    Dictionary<string, GameObject> pool = new Dictionary<string, GameObject>();
}
