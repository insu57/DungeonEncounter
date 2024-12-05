using Scriptable_Objects;
using UnityEngine;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private PlayerEquipmentData data;
        public PlayerEquipmentData Data => data;
    }
}
