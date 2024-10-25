using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public Button BtnPlay;
    public Button BtnSetting;
    public Button BtnExit;
    public GameObject PlayTab;
    public Button BtnBack;
    public Button BtnContinue;


    void Start()
    {
        BtnPlay.onClick.AddListener(OpenPlayTab);
        BtnBack.onClick.AddListener(ClosePlayTab);
        BtnContinue.onClick.AddListener(ChangeMainScene);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (PlayTab.activeSelf)
            {
                PlayTab.SetActive(false);
            }
        }
    }


    void OpenPlayTab()
    {
        PlayTab.SetActive(true);
    }
    void ClosePlayTab()
    {
        PlayTab.SetActive(false);
    }

    void ChangeMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

}
