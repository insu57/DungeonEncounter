using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData",
    menuName = "ScriptableObjects/ItemData", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private string type;
}
