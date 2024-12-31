using System;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponData weaponData;
        [SerializeField] private ParticleSystem particleMainCircle;
        [SerializeField] private ParticleSystem particleLight;
        public PlayerWeaponData WeaponData => weaponData;

        private void Awake()
        {
            if (particleMainCircle == null) return;
            var particleMain = particleMainCircle.main;
            particleMain.startColor = EnumManager.RarityToColor(weaponData.Rarity);
            particleMain = particleLight.main;
            particleMain.startColor = EnumManager.RarityToColor(WeaponData.Rarity);
        }
    }
}
