using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this for UI components

public class MinimapController : MonoBehaviour
{
    [SerializeField] Camera minimapCamera;
    [SerializeField] float zoomLevel = 15f;
    [SerializeField] LayerMask minimapLayers;

    [Header("Icons")]
    [SerializeField] GameObject enemyIconPrefab;
    [SerializeField] GameObject towerIconPrefab;
    [SerializeField] GameObject bossIconPrefab;
    [SerializeField] RectTransform iconContainer; // Change to RectTransform

    [Header("Updates")]
    [SerializeField] float updateInterval = 0.2f;

    private Dictionary<GameObject, GameObject> entityToIcon = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        if (minimapCamera == null)
        {
            Debug.LogError("Minimap camera not assigned!");
            return;
        }

        minimapCamera.orthographicSize = zoomLevel;
        minimapCamera.cullingMask = minimapLayers;

        StartCoroutine(UpdateMinimap());
    }

    IEnumerator UpdateMinimap()
    {
        while (true)
        {
            UpdateEnemyIcons();

            yield return new WaitForSeconds(updateInterval);
        }
    }
    void UpdateEnemyIcons()
    {
        // Clear old icons
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in entityToIcon)
        {
            if (pair.Key == null)
            {
                toRemove.Add(pair.Key);
            }
        }

        foreach (var key in toRemove)
        {
            Destroy(entityToIcon[key]);
            entityToIcon.Remove(key);
        }

        // Update enemy icons
        if (SpawnerController.Instance != null)
        {
            List<Enemy> enemies = SpawnerController.Instance.GetEnemies();
            foreach (Enemy enemy in enemies)
            {
                if (!entityToIcon.ContainsKey(enemy.gameObject))
                {
                    // Create new icon
                    GameObject iconPrefab = enemy.IsBoss() ? bossIconPrefab : enemyIconPrefab;
                    GameObject icon = Instantiate(iconPrefab, iconContainer);
                    entityToIcon.Add(enemy.gameObject, icon);
                }

                // Update icon position
                GameObject enemyIcon = entityToIcon[enemy.gameObject];
                if (enemyIcon != null)
                {
                    Vector3 worldPos = enemy.transform.position;
                    Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);

                    // Calculate position within the container
                    float posX = (viewportPos.x * iconContainer.rect.width) - (iconContainer.rect.width * 0.5f);
                    float posY = (viewportPos.y * iconContainer.rect.height) - (iconContainer.rect.height * 0.5f);

                    enemyIcon.transform.localPosition = new Vector3(posX, posY, 0);
                }
            }
        }
    }

    // Call this when towers are built
    public void RegisterTower(GameObject tower)
    {
        if (!entityToIcon.ContainsKey(tower))
        {
            GameObject icon = Instantiate(towerIconPrefab, iconContainer);
            entityToIcon.Add(tower, icon);

            // Update icon position
            Vector3 worldPos = tower.transform.position;
            Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);

            // Calculate position within the container
            float posX = (viewportPos.x * iconContainer.rect.width) - (iconContainer.rect.width * 0.5f);
            float posY = (viewportPos.y * iconContainer.rect.height) - (iconContainer.rect.height * 0.5f);

            icon.transform.localPosition = new Vector3(posX, posY, 0);
        }
    }
}