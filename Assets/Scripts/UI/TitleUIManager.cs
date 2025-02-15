using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    private Button _btnPlay;
    private Button _btnSetting;
    private Button _btnQuit;
    private GameObject _playTab;
    private Button _btnBack;
    private Button _btnContinue;
    //change to private
    private void Awake()
    {
        _btnPlay = transform.Find("ButtonPlay").GetComponent<Button>();
        _playTab = transform.Find("PlayTab").gameObject;
        _btnSetting = transform.Find("ButtonSetting").GetComponent<Button>();
        _btnQuit = transform.Find("ButtonQuit").GetComponent<Button>();
        
        //PlayTab
        _btnBack = _playTab.transform.Find("ButtonBack").GetComponent<Button>();
        _btnContinue = _playTab.transform.Find("ButtonContinue").GetComponent<Button>();
    }

    private void Start()
    {
        _btnPlay.onClick.AddListener(OpenPlayTab);
        _btnSetting.onClick.AddListener(OpenSettingTab);
        _btnQuit.onClick.AddListener(Application.Quit);
        
        //PlayTab
        _btnBack.onClick.AddListener(ClosePlayTab);
        _btnContinue.onClick.AddListener(ChangeMainScene);
        //AudioManager.Instance.PlayBGM(AudioManager.BGM.TitleBgm);
    }

    private void Update()
    {
        if (!Input.GetButtonDown("Cancel")) return;
        if (_playTab.activeSelf)
        {
            _playTab.SetActive(false);
        }
    }

    private void OpenSettingTab()
    {
        Debug.Log("BtnSetting");
    }
    
    private void OpenPlayTab()
    {
        _playTab.SetActive(true);
    }

    private void ClosePlayTab()
    {
        _playTab.SetActive(false);
    }

    private static void ChangeMainScene()
    {
        LoadingManager.LoadScene(LoadingManager.MainScene);
        //AudioManager.Instance.PlayBGM(AudioManager.BGM.MainBgm);
    }

}
