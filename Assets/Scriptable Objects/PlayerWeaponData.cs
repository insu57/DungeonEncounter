using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "PlayerWeaponData",
        menuName = "ScriptableObjects/PlayerWeaponData", order = int.MaxValue)]
    public class PlayerWeaponData : ScriptableObject
    {
       
        [SerializeField] private string weaponName;
        [SerializeField] private string description;
        [SerializeField] private WeaponType type;
        [SerializeField] private Rarity rarity;
        [SerializeField] private float attackValue;
        [SerializeField] private Sprite icon;
        public string WeaponName => weaponName;
        public string Description => description;
        public WeaponType Type => type;
        public Rarity Rarity => rarity;
        public float AttackValue => attackValue;
        public Sprite Icon => icon;
    }
}