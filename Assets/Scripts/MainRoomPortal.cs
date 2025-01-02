using UnityEngine;
using UnityEngine.SceneManagement;

public class MainRoomPortal : MonoBehaviour
{
    //일단 기본적인 씬 전환만
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject floatEnterStage;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floatEnterStage.SetActive(true);
            floatEnterStage.transform.position = transform.position + Vector3.back;
            if (Input.GetKeyDown(KeyCode.F))
            {
               //Stage Change(Scene)
               //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
               LoadingManager.LoadScene(LoadingManager.Stage1Scene);
            }
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
