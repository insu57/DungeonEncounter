using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> //Singleton Game Manager 싱글톤 게임 매니저
{
    public GameObject PanelPause;
    bool GamePaused;

    void Update()
    {
        //if(SceneManager.)
        if (Input.GetButtonDown("Cancel")) //ESC->Setting Window 설정창
        {
            if(!GamePaused) 
            {
                GamePaused = true;
                PanelPause.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                PanelPause.SetActive(false);
                GamePaused = false;
                Time.timeScale = 1f;
            }
        }
        /*
        if (GamePaused && )
        {

        }*/

    }
    
    
}
