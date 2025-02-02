using DG.Tweening;
using Player;
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
    }

    public void RetryStage()
    {
        _stageManager = FindObjectOfType<StageManager>();
        if (_stageManager)
        {
            _stageManager.ResetStage();
        }
        
        //
        
        GamePaused = false;
    }

    public void ReturnMainRoom()
    {
        LoadingManager.LoadScene(LoadingManager.MainScene);
        _stageManager = FindObjectOfType<StageManager>();
        if (_stageManager)
        {
            _stageManager.ResetStage();
        }
        GamePaused = false;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == LoadingManager.TitleScene)
        {
            Destroy(gameObject);
        }
    }
    
    public override void Awake()
    {
        base.Awake();
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerManager.OnPlayerDeath += HandlePlayerDeath;
        
        GamePaused = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnDestroy()
    {
        _playerManager.OnPlayerDeath -= HandlePlayerDeath;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
