using UnityEngine;

public class TowerSelectManager : MonoBehaviour
{
    public static TowerSelectManager Instance;
    public LayerMask mask;
    Ray ray;
    RaycastHit hit;

    [SerializeField] private GameObject selectedTower;
    private Tower thisTower;

    [SerializeField] Tower curTowerSelected;
    [SerializeField] Tower prevTowerSelected;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        SelectTower(); 
    }

    private void SelectTower()
    {
        if (BuildManager.Instance.GetIsDragging() == false) //lmb = select tower
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                thisTower = hit.collider.gameObject.GetComponentInParent<Tower>();
                selectedTower = hit.collider.gameObject;
                HighlightTower(thisTower);
            }
        }
        if (Input.GetMouseButtonDown(1) && selectedTower != null) //rmb = cancel selection
        {
            CancelSelectTower();
        }
    }

    private void HighlightTower(Tower newTower)
    {
        curTowerSelected = newTower;

        if (Input.GetMouseButtonDown(0))
        {

            if (prevTowerSelected != curTowerSelected)
            {
                if (prevTowerSelected != null) prevTowerSelected.SetColor(Color.white);
                prevTowerSelected = curTowerSelected;
            }

            if (thisTower.GetIsBuilding() == true) //prevent from selecting tower before it is built
            {
                newTower = null;
                selectedTower = null;
                return;
            }
            else
            {
                UIHandler.Instance.DisplayTowerInfo(newTower);
                curTowerSelected.SetColor(Color.cyan);
            }
        }
    }

    public void DeselectTower()
    {
        if (prevTowerSelected != null) prevTowerSelected.SetColor(Color.white);
    }

    public void CancelSelectTower()
    {
        UIHandler.Instance.HideTowerInfo();
        curTowerSelected.SetColor(Color.white);

        curTowerSelected = null;
        prevTowerSelected = null;
        thisTower = null;
        selectedTower = null;
    }

    public GameObject GetSelectedTower() { return selectedTower; }
}