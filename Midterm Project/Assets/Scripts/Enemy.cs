using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private int damageAmount = 10;
    private Transform player;
    private PlayerController playerController;
    private bool hasDamaged = false;  // Prevent multiple rapid damage
    private float damageTimer = 0.5f; // Time between possible damages
    private float lastDamageTime = 0f;
    private bool isDying = false; // Flag to prevent multiple death calls
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
            Debug.Log($"Enemy initialization - Player Found: {player != null}, PlayerController: {playerController != null}");
            // Log the collision components
            Debug.Log($"Enemy Collider: {GetComponent<Collider>() != null}");
            Debug.Log($"Enemy Rigidbody: {GetComponent<Rigidbody>() != null}");
            Debug.Log($"Player Collider: {playerObj.GetComponent<Collider>() != null}");
            Debug.Log($"Player Rigidbody: {playerObj.GetComponent<Rigidbody>() != null}");
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
        if (isDying) return; // Don't update if dying

        if (player != null)
        {
            // Move towards player
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
            transform.LookAt(player);

            // Reset damage flag after timer
            if (hasDamaged && Time.time > lastDamageTime + damageTimer)
            {
                hasDamaged = false;
            }

            // Check for very close proximity if collisions aren't working
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (!hasDamaged && distanceToPlayer < 1.5f)  // Adjust this value as needed
            {
                TryDamagePlayer();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDying) return;

        Debug.Log($"Collision detected with: {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        Debug.Log($"Trigger detected with: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    private void TryDamagePlayer()
    {
        if (!hasDamaged && playerController != null)
        {
            Debug.Log("Attempting to damage player");
            playerController.TakeDamage(damageAmount);
            hasDamaged = true;
            lastDamageTime = Time.time;
            Debug.Log($"Dealt {damageAmount} damage to player");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        AudioManager.Instance.PlaySFX("EnemyHit");
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}");

        if (currentHealth <= 0 && !isDying)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        Debug.Log("Enemy starting death sequence");

        // Disable components to prevent further interactions
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;

        // Notify the spawner before destruction
        if (WaveEnemySpawner.Instance != null)
        {
            WaveEnemySpawner.Instance.EnemyKilled();
            Debug.Log("WaveEnemySpawner notified of enemy death");
        }
        else
        {
            Debug.LogWarning("WaveEnemySpawner instance not found!");
        }

        // Use delayed destruction to ensure EnemyKilled processes first
        StartCoroutine(DelayedDestruction());
    }

    private System.Collections.IEnumerator DelayedDestruction()
    {
        // Wait for a frame to ensure EnemyKilled processes
        yield return new WaitForEndOfFrame();

        Debug.Log("Enemy destroyed!");
        Destroy(gameObject);
    }
}