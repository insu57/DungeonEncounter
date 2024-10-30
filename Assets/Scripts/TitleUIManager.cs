using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    //구조 수정
    private Button _btnPlay;
    private Button _btnSetting;
    private Button _btnExit;
    private GameObject _playTab;
    private Button _btnBack;
    private Button _btnContinue;

    private void Awake()
    {
        /* public->private인한 수정...
        _btnPlay = transform.GetChild(1);
        _btnSetting = GetComponentInChildren<Button>();
        _btnExit = GetComponentInChildren<Button>();
        _playTab = GameObject.Find("PlayTab");
        _btnBack = GameObject.Find("BackButton");
        */
    }

    private void Start()
    {
        _btnPlay.onClick.AddListener(OpenPlayTab);
        _btnBack.onClick.AddListener(ClosePlayTab);
        _btnContinue.onClick.AddListener(ChangeMainScene);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (_playTab.activeSelf)
            {
                _playTab.SetActive(false);
            }
        }
    }


    private void OpenPlayTab()
    {
        _playTab.SetActive(true);
    }

    private void ClosePlayTab()
    {
        _playTab.SetActive(false);
    }

    private void ChangeMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

}
