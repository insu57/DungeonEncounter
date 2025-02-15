using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIView : MonoBehaviour
    {
        [SerializeField] private Canvas canvasMain;
        //Game Menu
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button returnTitleBtn;
        [SerializeField] private Button quitGameBtn;
        
        //Player Info
        [Header("Player Info")]
        //직업 관련 추가 필요
        [SerializeField] private Image playerHealthBar;
        [SerializeField] private Image playerEnergyBar;
        //PlayerMenu...Inventory
        [Header("Player Menu")]
        [SerializeField] private TextMeshProUGUI jobText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        //PlayerDeath
        [Header("Player Death")]
        [SerializeField] private GameObject playerDeathMenu;        
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;
        public void TogglePause()
        {
            GameManager.Instance.TogglePause();
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Cursor.visible = pauseMenu.activeSelf;
            Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Confined;
        }

        public void TogglePlayerDeathMenu(bool isOpen)
        {
            if (isOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                playerDeathMenu.SetActive(true);
                pauseMenu.SetActive(false);
                retryButton.onClick.AddListener(GameManager.Instance.RetryStage);
                retryButton.onClick.AddListener(() => playerDeathMenu.SetActive(false));
                exitButton.onClick.AddListener(GameManager.Instance.ReturnMainRoom);
                exitButton.onClick.AddListener(()=> playerDeathMenu.SetActive(false));
                GameManager.Instance.HandlePlayerDeath();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
                playerDeathMenu.SetActive(false);
            }
            
        }
        
        public void UpdatePlayerHealthBar(float health, float maxHealth)
        {
            playerHealthBar.fillAmount = health / maxHealth;
            healthText.text = $"{health}/{maxHealth}";
            //Debug.Log($"{health}/{maxHealth}");
        }

        public void UpdatePlayerEnergyBar(float energy, float maxEnergy)
        {
            playerEnergyBar.fillAmount = energy / maxEnergy;
            energyText.text = $"{energy}/{maxEnergy}";
        }
        
        public void UpdatePlayerAttackValue(float attackValue)
        {
            attackText.text = $"{attackValue}";
        }

        public void UpdatePlayerDefenseValue(float defenseValue)
        {
            defenseText.text = $"{defenseValue}";
        }
        
        private void Awake()
        {
            resumeButton.onClick.AddListener(TogglePause);
            returnTitleBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.TogglePause();
                LoadingManager.LoadScene(LoadingManager.TitleScene);
            });
            quitGameBtn.onClick.AddListener(Application.Quit);
        }

        private void Update()
        {
            //PauseMenu 일시정지
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause(); //일시정지or재개 
            }

        }
    }
}
