using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class PlayerWorldUIPresenter
{
    private readonly PlayerManager _playerManager;
    private readonly WorldUIView _worldUIView;
    
    public PlayerWorldUIPresenter(PlayerManager player, WorldUIView view)
    {
        _playerManager = player;
        _worldUIView = view;

        _playerManager.OnFloatKey += HandleOnFloatKey;
        _playerManager.OnExitFloatKey += HandleOnExitFloatKey;
    }

    private void HandleOnFloatKey(FloatText textType, Vector3 position)
    {
        switch (textType)
        {
            case FloatText.Get:
                _worldUIView.SetFloatingKeyText("Get", position);
                break;
            case FloatText.Open:
                _worldUIView.SetFloatingKeyText("Open", position);
                break;
            case FloatText.Use:
                _worldUIView.SetFloatingKeyText("Use", position);
                break;
            default:
                break;
        }
    }

    private void HandleOnExitFloatKey()
    {
        _worldUIView.InactivateFloatingKey();
    }

    public void Dispose()
    {
        _playerManager.OnFloatKey -= HandleOnFloatKey;
        _playerManager.OnExitFloatKey -= HandleOnExitFloatKey;
    }
}
