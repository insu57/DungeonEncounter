using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class EquipmentDataAssign : ItemDataAssign
{
    [SerializeField] private PlayerEquipmentData equipmentData;

    public override IItemData GetItemData()
    {
        return equipmentData;
    }

    private void Awake()
    {
        SetParticleArray();
        SetParticleColor(equipmentData);
    }
}
