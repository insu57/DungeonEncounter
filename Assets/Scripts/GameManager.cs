using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> //�̱��� ���� �Ŵ��� 
{
    public GameObject PanelPause;
    
    bool GamePaused;

    void Update()
    {
        //if(SceneManager.)
        if (Input.GetButtonDown("Cancel")) //ESC ����â. 
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
