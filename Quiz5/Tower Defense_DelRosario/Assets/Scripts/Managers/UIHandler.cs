using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    [SerializeField] private GameObject escapePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Game Info")]
    [SerializeField] private Text waveText;
    [SerializeField] private Text goldAmountText;

    [Header("Tower Info")]
    [SerializeField] private GameObject towerInfoUI;
    [SerializeField] private Text towerInfoText;
    [SerializeField] private Text buildInfoText;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buildInfoText.gameObject.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        towerInfoUI.SetActive(false);

    }

    void Update()
    {
        OpenEscapePanel();
        UpdateGoldText();
        UpdateWaveText();
    }

    #region In-Game Panels
    private void OpenEscapePanel()
    {
        if (!escapePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeInHierarchy || controlsPanel.activeInHierarchy) return;
            Time.timeScale = 0;
            escapePanel.SetActive(true);
        }
        else if (escapePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseEscapePanel();
        }
    }

    private void CloseEscapePanel()
    {
        Time.timeScale = 1f;
        escapePanel.SetActive(false);
    }

    public void InGameSettingsButton()
    {
        escapePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void InGameControlsButton()
    {
        escapePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void BackButton()
    {
        if (escapePanel.activeInHierarchy)
        {
            CloseEscapePanel();
        }
        if (settingsPanel.activeInHierarchy)
        {
            settingsPanel.SetActive(false);
            escapePanel.SetActive(true);
        }
        if (controlsPanel.activeInHierarchy)
        {
            controlsPanel.SetActive(false);
            escapePanel.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
    #endregion

    #region Player / Game Info
    private void UpdateWaveText()
    {
        if (GameManager.Instance.GetWaveType == WaveType.Boss)
        {
            waveText.text = "Boss Wave";
        }
        else waveText.text = "Wave " + GameManager.Instance.GetWaveNumber.ToString();

        if (SpawnerController.enemiesAlive == 0)
        {
            waveText.text = "Get ready for the next wave!";
        }
    }

    private void UpdateGoldText()
    {
        goldAmountText.text = PlayerManager.Instance.GetGoldAmount.ToString();
    }

    public void ShowEndGamePanel(string panelName)
    {
        Time.timeScale = 0;
        if (panelName == "winPanel") winPanel.SetActive(true);
        if (panelName == "losePanel") losePanel.SetActive(true);
    }
    #endregion

    #region Tower Info
    public void ResetInfoText()
    {
        buildInfoText.text = "";
    }

    private void UpdateBuildInfo(string towerName)
    {
        buildInfoText.gameObject.SetActive(true);
        if (towerName == "Arrow")
        {
            buildInfoText.text = "Price: 100 gold" +
                "\nDamage: 30-50" + "\nFire rate: Fast" + "\nBuild time: 5s" +
                "\nLong range, single target. Ground and Flying monsters.";
        }
        if (towerName == "Cannon")
        {
            buildInfoText.text = "Price: 200 gold" +
                "\nDamage: 20-25" + "\nFire rate: Slow" + "\nBuild time: 7.5s" +
                "\nShort range, splash target. Ground monsters only.";
        }
        if (towerName == "Ice")
        {
            buildInfoText.text = "Price: 500 gold" +
                "\nDamage: 10-13" + "\nFire rate: Medium" + "\nBuild time: 15s" +
                "\nMedium range, splash target with 'Chilled' effect. Can target flying monsters.";
        }
        if (towerName == "Fire")
        {
            buildInfoText.text = "Price: 500 gold" +
                "\nDamage: 10-13" + "\nFire rate: Medium" + "\nBuild time: 15s" +
                "\nMedium range, splash target with 'Burning' effect. Can target flying monsters.";
        }
    }

    public void HideBuildInfo()
    {
        buildInfoText.gameObject.SetActive(false);
    }

    public IEnumerator NotEnoughGold()
    {
        HideTowerInfo();
        buildInfoText.text = "Not enough gold!";
        yield return new WaitForSeconds(3);
        ResetInfoText();
    }

    public IEnumerator CantBuildHere()
    {
        buildInfoText.text = "Can't build here!";
        yield return new WaitForSeconds(3);
        ResetInfoText();
    }

    public void DisplayTowerInfo(Tower tower)
    {
        towerInfoUI.SetActive(true);
        if (tower.name.Contains("Crossbow"))
        {
            towerInfoText.text = "Crossbow Tower: Tier " +
                tower.GetTierNumber() + "\nDamage: " + tower.GetDamage() +
                "\nFire Rate: " + tower.GetFireRate() +
                "\nRange: " + tower.GetRange();
        }
        if (tower.name.Contains("Cannon"))
        {
            towerInfoText.text = "Cannon Tower: Tier " +
                tower.GetTierNumber() + "\nDamage: " + tower.GetDamage() +
                "\nFire Rate: " + tower.GetFireRate() +
                "\nRange: " + tower.GetRange();
        }
        if (tower.name.Contains("Crystal"))
        {
            towerInfoText.text = "Ice Tower: Tier " +
                tower.GetTierNumber() + "\nDamage: " + tower.GetDamage() +
                "\nFire Rate: " + tower.GetFireRate() +
                "\nRange: " + tower.GetRange() +
                "\nChilled Debuff Value: " + tower.GetChilledValue();
        }
        if (tower.name.Contains("Fire"))
        {
            towerInfoText.text = "Fire Tower: Tier " +
                tower.GetTierNumber() + "\nDamage: " + tower.GetDamage() +
                "\nFire Rate: " + tower.GetFireRate() +
                "\nRange: " + tower.GetRange() +
                "\nBurning Damage: " + tower.GetBurningDamage();
        }
    }

    public void HideTowerInfo()
    {
        towerInfoUI.SetActive(false);
    }
    #endregion

    #region Buy Buttons
    public void BuyArrowTowerButton()
    {
        BuildManager.Instance.SpawnTower(0);
        TowerSelectManager.Instance.DeselectTower();
        UpdateBuildInfo("Arrow");
    }

    public void BuyCannonTowerButton()
    {
        BuildManager.Instance.SpawnTower(1);
        TowerSelectManager.Instance.DeselectTower();
        UpdateBuildInfo("Cannon");
    }

    public void BuyIceTowerButton()
    {
        BuildManager.Instance.SpawnTower(2);
        TowerSelectManager.Instance.DeselectTower();
        UpdateBuildInfo("Ice");
    }

    public void BuyFireTowerButton()
    {
        BuildManager.Instance.SpawnTower(3);
        TowerSelectManager.Instance.DeselectTower();
        UpdateBuildInfo("Fire");
    }
    #endregion
}