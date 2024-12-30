using Scriptable_Objects;
using UnityEngine;

namespace Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private PlayerEquipmentData data;
        [SerializeField] private ParticleSystem particleMainCircle;
        [SerializeField] private ParticleSystem particleLight;
        public PlayerEquipmentData Data => data;

        private void Awake()
        {
            //이펙트 색상
            var particleMain = particleMainCircle.main;
            particleMain.startColor = EnumManager.RarityToColor(data.Rarity);
            particleMain = particleLight.main;
            particleMain.startColor = EnumManager.RarityToColor(data.Rarity);   
        }
    }
}
