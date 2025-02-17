// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;  // Reference to the HealthBar script

    [Header("Shooting Settings")]
    [SerializeField] private GameObject[] bulletPrefabs;
    [SerializeField] private Transform[] nozzlePoints;

    AudioManagers audioManager;

    private int currentNozzle = 0;
    private float[] fireRates = { 0.4f, 0.3f, 0.2f, 0.1f };
    private float nextFireTime = 0f;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManagers>();
    }

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleNozzleSwitch();
        HandleShooting();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Player taking damage: {damage}. Current health: {currentHealth}");
        currentHealth -= damage;
      //  audioManager.PlaySFX(audioManager.PlayerDeath);

        if (healthBar != null)
        {
            Debug.Log($"Updating health bar to: {currentHealth}");
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogError("HealthBar reference is missing!");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
       // audioManager.PlaySFX(audioManager.PlayerDeath);
        // Disable player control
        enabled = false;

        // Show game over screen
        GameManager.Instance.ShowGameOver();

        // Optionally disable enemy spawning
        if (WaveEnemySpawner.Instance != null)
        {
            WaveEnemySpawner.Instance.enabled = false;
        }
    }


void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleNozzleSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentNozzle = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentNozzle = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentNozzle = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentNozzle = 3;
    }

    void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime && currentNozzle < nozzlePoints.Length)
        {
            if (currentNozzle == 1 && nozzlePoints.Length >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Transform currentNozzleTransform = nozzlePoints[i];
                    GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                   currentNozzleTransform.position,
                                                   currentNozzleTransform.rotation);
                }
                audioManager.PlaySFX(audioManager.BulletSounds);
                nextFireTime = Time.time + fireRates[currentNozzle];
                
            }
            else if (currentNozzle == 2 && nozzlePoints.Length >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Transform currentNozzleTransform = nozzlePoints[i];
                    GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                   currentNozzleTransform.position,
                                                   currentNozzleTransform.rotation);
                }
                nextFireTime = Time.time + fireRates[currentNozzle];
                audioManager.PlaySFX(audioManager.BulletSounds);
            }
            else if (currentNozzle == 3 && nozzlePoints.Length >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform currentNozzleTransform = nozzlePoints[i];
                    GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                   currentNozzleTransform.position,
                                                   currentNozzleTransform.rotation);
                }
                nextFireTime = Time.time + fireRates[currentNozzle];
                audioManager.PlaySFX(audioManager.BulletSounds);
            }
            else
            {
                Transform currentNozzleTransform = nozzlePoints[currentNozzle];
                GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                currentNozzleTransform.position,
                                                currentNozzleTransform.rotation);
                nextFireTime = Time.time + fireRates[currentNozzle];
                audioManager.PlaySFX(audioManager.BulletSounds);
            }
        }
    }
}

