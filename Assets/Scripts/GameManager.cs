using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> //Singleton Game Manager 싱글톤 게임 매니저
{
    public bool GamePaused { get; private set; }
    private PlayerManager _playerManager;
    private StageManager _stageManager;
    public void TogglePause()
    {
        GamePaused = !GamePaused;
    }

    public void HandlePlayerDeath()
    {
        GamePaused = true;
        _stageManager = FindObjectOfType<StageManager>();
        if (_stageManager)
        {
            _stageManager.ResetStage();
        }
    }

    public void RetryStage()
    {
        _playerManager.transform.position = Vector3.zero;
        _playerManager.ResetStat();
        GamePaused = false;
    }

    public void ReturnMainRoom()
    {
        LoadingManager.LoadScene(LoadingManager.MainScene);
        _playerManager.transform.position = Vector3.zero;
        _playerManager.ResetStat();
        GamePaused = false;
    }
    public override void Awake()
    {
        base.Awake();
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerManager.OnPlayerDeath += HandlePlayerDeath;
    }
    private void Update()
    {
        if(GamePaused) 
        {
            Time.timeScale = 0f;
            DOTween.PauseAll();
        }
        else
        {
            Time.timeScale = 1f;
            DOTween.PlayAll();
        }
    }
}
