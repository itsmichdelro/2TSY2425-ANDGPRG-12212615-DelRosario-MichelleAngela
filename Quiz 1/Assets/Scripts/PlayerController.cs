using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 0.5f;    // Controls how fast the player moves

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI planetNameText;    // Reference to UI text that shows planet names

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 4f;    // How close player needs to be to detect planets
    private SphereCollider triggerCollider;

    private void Awake()
    {
        // Set up detection sphere around player
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = detectionRadius;

        if (planetNameText == null)
        {
            Debug.LogError("Planet Name Text reference is missing!");
        }
    }

    private void Update()
    {
        HandleMovement();    // Process movement input every frame
    }

    // Handles WASD/Arrow key movement
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");    // A/D or Left/Right arrows
        float verticalInput = Input.GetAxisRaw("Vertical");        // W/S or Up/Down arrows

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    // Called when player enters planet's detection zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Planet") && planetNameText != null)
        {
            string planetName = other.gameObject.name.Replace("(Clone)", "").Trim();
            planetNameText.text = $"Approaching: {planetName}";
        }
    }

    // Called when player leaves planet's detection zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Planet") && planetNameText != null)
        {
            planetNameText.text = "";
        }
    }
}