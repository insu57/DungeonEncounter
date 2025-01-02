using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    public const string TitleScene = "TitleScene";
    public const string MainScene = "MainScene";
    public const string Stage1Scene = "Stage1Scene";
    public const string LoadingScene = "LoadingScene";
    //?
    
    private static string _nextScene;
    
    
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene(LoadingScene);
    }

    private IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextScene);
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = false;
            float timer = 0.0f;
            while (!asyncLoad.isDone)
            {
                yield return null;
                timer += Time.deltaTime;
                if (asyncLoad.progress < 0.9f)
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, asyncLoad.progress, timer);
                    if (progressBar.fillAmount >= asyncLoad.progress)
                    {
                        timer = 0.0f;
                    }
                }
                else
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                    if (Mathf.Approximately(progressBar.fillAmount, 1.0f))
                    {
                        asyncLoad.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }

        
    }
}
