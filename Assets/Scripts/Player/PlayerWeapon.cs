using System;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        public ItemData ItemData => itemData;
        public ItemTypes ItemType => itemData.ItemType;
    }
}
