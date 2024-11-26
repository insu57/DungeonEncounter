using System;

namespace UI
{
    public interface IPlayerUIView 
    {
        public void UpdatePlayerHealthBar(float health, float maxHealth);
        public void UpdatePlayerEnergyBar(float energy, float maxEnergy);
        public void UpdatePlayerAttackValue(float value);
        public void UpdatePlayerDefenseValue(float value);
    }
}
