
using Player;
using UnityEngine;

namespace UI
{
    public class PlayerUIPresenter
    {
        private readonly PlayerManager _model;
        private readonly IPlayerUIView _uiView;

        public PlayerUIPresenter(PlayerManager model, IPlayerUIView uiView)
        {
            _model = model;
            _uiView = uiView;

            _model.OnStatChanged += HandleStatChanged;
        }
        
        private void HandleStatChanged(PlayerStatTypes statTypes, float value)
        {
            Debug.Log("EVENT HANDLE");
            switch(statTypes)
            {
                case PlayerStatTypes.Health:
                    float maxHealth = _model.GetStat(PlayerStatTypes.MaxHealth);
                    _uiView.UpdatePlayerHealthBar(value, maxHealth);
                    break;
                case PlayerStatTypes.MaxHealth:
                    float health = _model.GetStat(PlayerStatTypes.Health);
                    _uiView.UpdatePlayerHealthBar(health, value);
                    break;
                case PlayerStatTypes.Energy:
                    float maxEnergy = _model.GetStat(PlayerStatTypes.MaxEnergy);
                    _uiView.UpdatePlayerEnergyBar(value, maxEnergy);
                    break;
                case PlayerStatTypes.MaxEnergy:
                    float energy = _model.GetStat(PlayerStatTypes.Energy);
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
            _model.OnStatChanged -= HandleStatChanged;
        }
        
    }
}
