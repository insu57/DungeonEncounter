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

        private void TogglePause()
        {
            GameManager.Instance.TogglePause();
            pauseMenu.SetActive(!pauseMenu.activeSelf);
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
        
        //플레이어 정보 창일때만 호출하게 수정필요
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
