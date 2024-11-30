using System;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponData weaponData;
        public PlayerWeaponData WeaponData => weaponData;
    }
}
