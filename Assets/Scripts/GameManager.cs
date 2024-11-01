using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager> //Singleton Game Manager 싱글톤 게임 매니저
{
    public GameObject panelPause;
    private bool _gamePaused;

    public override void Awake()
    {
        base.Awake();
        //
    }
    private void Update()
    {
        //Debug.Log(SceneManager.sceneCount); //????
    
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
