using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerWeaponData",
        menuName = "Scriptable Objects/PlayerWeaponData", order = int.MaxValue)]
    public class PlayerWeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName;
        [SerializeField] private string description;
        [SerializeField] private string type;
        [SerializeField] private string rarity;
        [SerializeField] private float damage;

        public string WeaponName => weaponName;
        public string Description => description;
        public string Type => type;
        public string Rarity => rarity;
        public float Damage => damage;
    }
}