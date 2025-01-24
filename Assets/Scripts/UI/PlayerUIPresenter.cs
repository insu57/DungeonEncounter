
using Player;
using UnityEngine;

namespace UI
{
    public class PlayerUIPresenter

    {
        private readonly PlayerManager _playerManager;
        private readonly InventoryManager _inventoryManager;
        private readonly PlayerUIView _uiView;
        
        
        public PlayerUIPresenter(PlayerManager playerManager ,PlayerUIView uiView) //presenter
        {
            _playerManager = playerManager; //PlayerDataModel
            _uiView = uiView; //view

            _playerManager.OnStatChanged += HandleStatChanged;
            _playerManager.OnPlayerDeath += HandlePlayerDeath;
            //init stat
            float health = _playerManager.GetStat(PlayerStatTypes.Health);
            float maxHealth = _playerManager.GetStat(PlayerStatTypes.MaxHealth);
            float energy = _playerManager.GetStat(PlayerStatTypes.Energy);
            float maxEnergy = _playerManager.GetStat(PlayerStatTypes.MaxEnergy);
            float attack = _playerManager.GetStat(PlayerStatTypes.AttackValue);
            float defense = _playerManager.GetStat(PlayerStatTypes.DefenseValue);

            _uiView.UpdatePlayerHealthBar(health, maxHealth);
            _uiView.UpdatePlayerEnergyBar(energy, maxEnergy);
            _uiView.UpdatePlayerAttackValue(attack);
            _uiView.UpdatePlayerDefenseValue(defense);
            
        }

        private void HandlePlayerDeath()
        {
            _uiView.TogglePlayerDeathMenu(true);
        }
        
        private void HandleStatChanged(PlayerStatTypes statTypes, float value)
        {
            switch (statTypes)
            {
                case PlayerStatTypes.Health:
                    float maxHealth = _playerManager.GetFinalStat(PlayerStatTypes.MaxHealth);
                    _uiView.UpdatePlayerHealthBar(value, maxHealth);
                    break;
                case PlayerStatTypes.MaxHealth:
                    float health = _playerManager.GetStat(PlayerStatTypes.Health);
                    _uiView.UpdatePlayerHealthBar(health, value);
                    break;
                case PlayerStatTypes.Energy:
                    float maxEnergy = _playerManager.GetFinalStat(PlayerStatTypes.MaxEnergy);
                    _uiView.UpdatePlayerEnergyBar(value, maxEnergy);
                    break;
                case PlayerStatTypes.MaxEnergy:
                    float energy = _playerManager.GetStat(PlayerStatTypes.Energy);
                    _uiView.UpdatePlayerEnergyBar(energy, value);
                    break;
                case PlayerStatTypes.AttackValue:
                    _uiView.UpdatePlayerAttackValue(value);
                    break;
                case PlayerStatTypes.DefenseValue:
                    _uiView.UpdatePlayerDefenseValue(value);
                    break;
                default:
                    break;
            }
        }

        
        public void Dispose()
        {
            _playerManager.OnStatChanged -= HandleStatChanged;
        }
    }
}
