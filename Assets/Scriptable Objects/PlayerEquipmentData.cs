using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerEquipmentData",
    menuName = "ScriptableObjects/PlayerEquipmentData", order = int.MaxValue)]
public class PlayerEquipmentData : ScriptableObject
{
    [SerializeField] private string equipmentName;
    [SerializeField] private string description;
    [SerializeField] private string type;
    [SerializeField] private string rarity;

    public string EquipmentName => equipmentName;
    public string Description => description;
    public string Type => type;
    public string Rarity => rarity;

}
