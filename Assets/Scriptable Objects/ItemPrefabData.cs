using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;
[CreateAssetMenu(fileName = "ItemPrefabData", menuName = "ItemPrefabData", order = 1)]
public class ItemPrefabData : ScriptableObject
{
    [Serializable]
    public class WeaponItemMapping
    {
        public PlayerWeaponData weaponData;
        public GameObject prefab;
    }

    [Serializable]
    public class EquipmentItemMapping
    {
        public PlayerEquipmentData equipmentData;
        public GameObject prefab;
    }

    [Serializable]
    public class ConsumableItemMapping
    {
        public ConsumableItemData consumableData;
        public GameObject prefab;
    }

    public List<WeaponItemMapping> weaponPrefabs;
    public List<EquipmentItemMapping> equipmentPrefabs;
    public List<ConsumableItemMapping> consumablePrefabs;

    public GameObject GetWeaponPrefab(PlayerWeaponData data)
    {
        return weaponPrefabs.Find(x => x.weaponData == data)?.prefab;
    }

    public GameObject GetEquipmentPrefab(PlayerEquipmentData data)
    {
        return equipmentPrefabs.Find(x => x.equipmentData == data)?.prefab;
    }

    public GameObject GetConsumablePrefab(ConsumableItemData data)
    {
        return consumablePrefabs.Find(x => x.consumableData == data)?.prefab;
    }
}
