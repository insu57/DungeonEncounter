using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> //Singleton Game Manager 싱글톤 게임 매니저
{
    public GameObject panelPause;
    private bool _gamePaused;
    public bool GamePaused => _gamePaused;
    private PlayerManager _playerManager;
    private bool _playerDead;
    
    public override void Awake()
    {
        base.Awake();
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerDead = false;
    }
    private void Update()
    {
        //if(_playerDead)
        
        
        if (!Input.GetButtonDown("Cancel")) return; //ESC->Setting Window 설정창
        if(!_gamePaused) 
        {
            _gamePaused = true;
            panelPause.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            panelPause.SetActive(false);
            _gamePaused = false;
            Time.timeScale = 1f;
        }
        
        

    }
    
    
}
