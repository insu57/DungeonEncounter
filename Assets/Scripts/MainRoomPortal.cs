using System;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainRoomPortal : MonoBehaviour
{
    //일단 기본적인 씬 전환만
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject floatEnterStage;
    
    private PlayerManager _player;
    private float _distance;
    private void Awake()
    {
        _player = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        _distance = Vector3.Distance(transform.position, _player.transform.position);//1.5f이내 일 때 
        if (_distance <= 1.5f)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                LoadingManager.LoadScene(LoadingManager.Stage1Scene);
                floatEnterStage.SetActive(false);
                _player.transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floatEnterStage.SetActive(true);
            floatEnterStage.transform.position = transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floatEnterStage.SetActive(false);
        }
    }
}
