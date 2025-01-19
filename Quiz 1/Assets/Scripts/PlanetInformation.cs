using UnityEngine;
using TMPro;

public class PlanetInformation : MonoBehaviour
{
    [Header("Planet Settings")]
    [SerializeField] private string planetName;              // Name of the planet
    [SerializeField] private float detectionRadius = 1.5f;    // How close player needs to be to see planet name

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI uiText;        // Reference to UI text component

    private Transform player;
    private SphereCollider triggerCollider;                 // Trigger collider for detection

    void Awake()
    {
        // Add trigger collider for detection
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = detectionRadius;

        // If planetName is not set, use object name
        if (string.IsNullOrEmpty(planetName))
        {
            planetName = gameObject.name.Replace("(Clone)", "").Trim();
        }
    }

    void Start()
    {
        // Find player in scene
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError($"Player not found! Make sure player object has 'Player' tag.");
        }

        // Check UI reference
        if (uiText == null)
        {
            Debug.LogError($"UI Text not assigned for {planetName}! Assign in inspector.");
        }
        else
        {
            uiText.gameObject.SetActive(false);
        }
    }

    // Called when player enters detection radius
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && uiText != null)
        {
            uiText.text = $"Approaching: {planetName}";
            uiText.gameObject.SetActive(true);
        }
    }

    // Called when player is within detection radius
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && uiText != null)
        {
            // Optional: Update distance or additional information while player is near
            float distance = Vector3.Distance(transform.position, player.position);
            uiText.text = $"Approaching: {planetName}\nDistance: {distance:F1} units";
        }
    }

    // Called when player exits detection radius
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    // Visualize detection radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}