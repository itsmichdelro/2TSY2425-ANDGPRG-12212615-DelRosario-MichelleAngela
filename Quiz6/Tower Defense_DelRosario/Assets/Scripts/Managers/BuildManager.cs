using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Tower Info")]
    private float buildableOffsetY = 2f;
    private float maxBuildableOffsetY = 2.5f;
    [SerializeField] private int towerCost;
    private bool isDragging;

    [SerializeField] GameObject draggableTower;
    [SerializeField] Tower tempTower;
    [SerializeField] GameObject[] towerPrefabs;
    // 0 - arrow, 1 - cannon, 2 - ice, 3 - fire

    [Header("Raycast Info")]
    Ray ray;
    RaycastHit hit; //which object is hit
    [SerializeField] RaycastHit[] allObject;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    { 
        PlaceTower(); //place tower in game scene
    }

    #region Building
    public void SpawnTower(int index) //so we can assign a button to spawn tower in build ui
    {
        isDragging = true;
        UIHandler.Instance.HideTowerInfo();
        GameObject towerToBuild = (GameObject)Instantiate(towerPrefabs[index]);
        draggableTower = towerToBuild;
        tempTower = towerToBuild.GetComponent<Tower>();
        towerCost = tempTower.GetTowerPrice();
    }

    void PlaceTower()
    {
        if (draggableTower != null)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            allObject = Physics.RaycastAll(ray); // all object

            //singular object
            if (Physics.Raycast(ray, out hit)) // out hit = if someone is being hit; retrieves what object is hit; assigns that info into this variable
            {
                draggableTower.transform.position = SnapToGrid(hit.point); //check height of specific hit point to determine if it is buildable area

                if (hit.point.y > buildableOffsetY && hit.point.y < maxBuildableOffsetY)
                {
                    tempTower.Buildable();
                    BuildTower();        
                }
                else //cant build
                {
                    CantBuildTower();
                }

                if (Input.GetMouseButtonDown(1)) // if rmb pressed, cancel the build
                {
                    CancelBuild();
                }
            }
        }
    }

    private void BuildTower()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) // can build
        {
            if (PlayerManager.Instance.GetGoldAmount >= towerCost)
            {
                isDragging = false;
                tempTower.Build();

                FindObjectOfType<AudioManager>().PlaySound("buildTower");
                PlayerManager.Instance.DecreaseGold(towerCost);
                UIHandler.Instance.ResetInfoText();
                UIHandler.Instance.HideBuildInfo();

                draggableTower = null; //null = it has already been placed in the world; remove it from the mouse pointer
                tempTower = null;
            }
            else // not enough gold
            {
                StartCoroutine(UIHandler.Instance.NotEnoughGold());
                return;
            }
        }
    }

    private void CancelBuild()
    {
        isDragging = false;
        Destroy(draggableTower);
        draggableTower = null;
        tempTower = null;
        UIHandler.Instance.ResetInfoText();
    }

    private void CantBuildTower()
    {
        tempTower.NonBuildable();

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(UIHandler.Instance.CantBuildHere());
        }
    }
    private Vector3 SnapToGrid(Vector3 towerPos)
    {
        return new Vector3(
            Mathf.Round(towerPos.x),
            towerPos.y, 
            Mathf.Round(towerPos.z)
            );
    }
    #endregion

    public bool GetIsDragging() { return isDragging; }
}
