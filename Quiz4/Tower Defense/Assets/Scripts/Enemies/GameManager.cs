using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Objects")]
    [SerializeField] GameObject crystalCore;
    [SerializeField] Camera mainCamera;

    [Header("Game Settings")]
    [SerializeField] int coreHealth = 20;
    [SerializeField] int maxCoreHealth = 20;
    [SerializeField] int playerGold = 500;
    [SerializeField] float cameraSpeed = 20f;
    [SerializeField] float cameraEdgeThreshold = 20f;

    [Header("UI References")]
    [SerializeField] Text waveText;
    [SerializeField] Text goldText;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject minimapPanel;

    [Header("Camera Settings")]
    [SerializeField] float cameraZoomSpeed = 5f;
    [SerializeField] float minZoom = 10f;
    [SerializeField] float maxZoom = 100f;

    public GameObject CrystalCore { get { return crystalCore; } }

    public string MainMenuScene { get; private set; }

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize UI
        UpdateGoldUI();
        UpdateHealthUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver)
        {
            HandleCameraMovement();
        }
    }
    private void HandleCameraMovement()
    {
        // Existing movement code
        Vector3 pos = mainCamera.transform.position;
        float xMovement = 0f;
        float zMovement = 0f;

        // Arrow key movement
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            xMovement = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            xMovement = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            zMovement = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            zMovement = -1;
        }

        // Mouse edge movement
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < cameraEdgeThreshold)
        {
            xMovement = -1;
        }
        else if (mousePos.x > Screen.width - cameraEdgeThreshold)
        {
            xMovement = 1;
        }

        if (mousePos.y < cameraEdgeThreshold)
        {
            zMovement = -1;
        }
        else if (mousePos.y > Screen.height - cameraEdgeThreshold)
        {
            zMovement = 1;
        }

        // Apply movement
        if (xMovement != 0 || zMovement != 0)
        {
            Vector3 movement = new Vector3(xMovement, 0, zMovement).normalized;
            mainCamera.transform.position += movement * cameraSpeed * Time.deltaTime;

            // Clamp camera position
            float minX = -10f, maxX = 50f, minZ = -10f, maxZ = 50f;
            float newX = Mathf.Clamp(mainCamera.transform.position.x, minX, maxX);
            float newZ = Mathf.Clamp(mainCamera.transform.position.z, minZ, maxZ);
            mainCamera.transform.position = new Vector3(newX, mainCamera.transform.position.y, newZ);
        }

        // Add zoom functionality
        float zoomDelta = 0f;

        // Q to zoom in, E to zoom out
        if (Input.GetKey(KeyCode.Q))
        {
            zoomDelta = -1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            zoomDelta = 1f;
        }

        // Apply zoom if needed
        if (zoomDelta != 0)
        {
            // If using perspective camera
            if (!mainCamera.orthographic)
            {
                // Change the field of view for perspective camera
                float newFOV = mainCamera.fieldOfView + zoomDelta * cameraZoomSpeed * Time.deltaTime;
                mainCamera.fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
            }
            else
            {
                // Change orthographic size for orthographic camera
                float newSize = mainCamera.orthographicSize + zoomDelta * cameraZoomSpeed * Time.deltaTime;
                mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    public int GetGold()
    {
        return playerGold;
    }

    public void DamageCrystalCore(int damage)
    {
        coreHealth -= damage;

        if (coreHealth <= 0)
        {
            coreHealth = 0;
            GameOver();
        }

        UpdateHealthUI();
    }

    public int GetMaxCoreHealth()
    {
        return maxCoreHealth;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + playerGold;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)coreHealth / maxCoreHealth;
        }
    }

    public void UpdateWaveUI(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + waveNumber;
        }
    }

    private void GameOver()
    {
        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over!");
    }

    public void GameWon()
    {
        isGameOver = true;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Debug.Log("Victory!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
}