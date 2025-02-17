using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private HealthBar healthBar;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject[] bulletPrefabs;
    [SerializeField] private Transform[] nozzlePoints;

    private Rigidbody2D rb;
    AudioManager audioManager;
    private Vector2 movement;
    private Vector2 mousePosition;
    private Camera cam;
    private Animator animator; // Add reference to Animator

    private int currentNozzle = 0;
    private float[] fireRates = { 0.4f, 0.3f, 0.2f, 0.1f };
    private float nextFireTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        cam = Camera.main;

       
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Get health bar reference
        if (UIManager.Instance != null)
        {
            healthBar = UIManager.Instance.GetHealthBar();
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
                healthBar.SetHealth(maxHealth);
            }
            else
            {
                Debug.LogError("Failed to get HealthBar from UIManager!");
            }
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
    }

    void Update()
    {
        // Only process input if player is alive
        if (currentHealth > 0)
        {
            // Input handling
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Correct mouse position code
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

            HandleNozzleSwitch();
            HandleShooting();
        }
    }

    void FixedUpdate()
    {
        // Only move if player is alive
        if (currentHealth > 0)
        {
            // Movement
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

            // Rotation
            Vector2 lookDirection = mousePosition - rb.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("PlayerDeath");
        }

        // Get fresh reference to health bar if needed
        if (healthBar == null && UIManager.Instance != null)
        {
            healthBar = UIManager.Instance.GetHealthBar();
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogError("HealthBar is null when taking damage!");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void OnEnable()
    {
        // Reset health when the component is enabled (which happens on scene load)
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    private void Die()
    {
        Debug.Log("Player died!");

        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger("IsDead");
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }

        // Disable player movement and physics
        enabled = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Show game over through UIManager only
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogError("UIManager instance not found!");
        }

        // Stop wave spawning
        if (WaveEnemySpawner.Instance != null)
        {
            WaveEnemySpawner.Instance.enabled = false;
        }
        else
        {
            Debug.LogWarning("WaveEnemySpawner instance not found!");
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
                AudioManager.Instance.PlaySFX("BulletSounds");
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
                AudioManager.Instance.PlaySFX("BulletSounds");
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
                AudioManager.Instance.PlaySFX("BulletSounds");
            }
            else
            {
                Transform currentNozzleTransform = nozzlePoints[currentNozzle];
                GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                currentNozzleTransform.position,
                                                currentNozzleTransform.rotation);
                nextFireTime = Time.time + fireRates[currentNozzle];
                AudioManager.Instance.PlaySFX("BulletSounds");
            }
        }
    }
}