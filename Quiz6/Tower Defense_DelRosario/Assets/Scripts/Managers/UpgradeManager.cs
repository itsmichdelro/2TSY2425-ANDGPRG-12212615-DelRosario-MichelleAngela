using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public LayerMask mask;
    Ray ray;
    RaycastHit hit;

    [Header("Upgrade Info")]
    [SerializeField] GameObject towerToUpgrade;
    [SerializeField] Tower tempTower;
    private Tower thisTower;
    private Tier tierNum;
    private int upgradeCost;
    private bool showInfo = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        towerToUpgrade = TowerSelectManager.Instance.GetSelectedTower();

        if (showInfo == true)
        {
            UIHandler.Instance.ShowUpgradeInfo(tierNum);
        }
    }

    public void SelectTower()
    {
        if (towerToUpgrade != null && BuildManager.Instance.GetIsDragging() == false)
        {
            UIHandler.Instance.HideBuildInfo();
            tempTower = towerToUpgrade.GetComponent<Tower>();
            upgradeCost = tempTower.GetUpgradePrice();
            tierNum = tempTower.GetTierNumber();
            thisTower = tempTower;

            showInfo = true;
        }
        if (Input.GetMouseButtonDown(1) && tempTower != null) //rmb = cancel selection
        {
            CancelSelectTower();
        }
    }

    public void CancelSelectTower()
    {
        showInfo = false;
        UIHandler.Instance.HideUpgradeInfo();
        tempTower.SetColor(Color.white);
        tempTower = null;
        towerToUpgrade = null;
    }
    private void ReplacePrefab()
    {
        if (towerToUpgrade != null)
        {
            Transform towerTransform = towerToUpgrade.transform;

            // identify tier of towerToUpgrade
            tierNum = towerToUpgrade.GetComponent<Tower>().GetTierNumber();
            if (tierNum == Tier.One)
            {
                GameObject currentTower = towerTransform.GetChild(0).gameObject;
                GameObject nextTower = towerTransform.GetChild(1).gameObject;

                currentTower.SetActive(false);
                nextTower.SetActive(true);
                tempTower.SetTierNumber(Tier.Two);
                tempTower.UpdateTurretAndProjectileStartingPoint();
            }
            if (tierNum == Tier.Two)
            {
                GameObject currentTower = towerTransform.GetChild(1).gameObject;
                GameObject nextTower = towerTransform.GetChild(2).gameObject;

                currentTower.SetActive(false);
                nextTower.SetActive(true);
                tempTower.SetTierNumber(Tier.Three);
                tempTower.UpdateTurretAndProjectileStartingPoint();
            }
            if (tierNum == Tier.Three)
            {
                Debug.Log("already at max");
            }
        }

        tempTower = null;
        towerToUpgrade = null;
    }

    public void UpgradeTower()
    {
        showInfo = false;
        ReplacePrefab();
        thisTower.UpgradeStats();

        FindObjectOfType<AudioManager>().PlaySound("upgradeTower");
        PlayerManager.Instance.DecreaseGold(upgradeCost);
    }

    public int GetUpgradeCost { get { return upgradeCost; } }
}