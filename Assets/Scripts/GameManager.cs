using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> //Singleton Game Manager 싱글톤 게임 매니저
{
    public bool GamePaused { get; private set; }
    private PlayerManager _playerManager;
    private bool _playerDead;

    public void TogglePause()
    {
        GamePaused = !GamePaused;
    }
    
    public override void Awake()
    {
        base.Awake();
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerDead = false;
    }
    private void Update()
    {
        //if(_playerDead)
        
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
