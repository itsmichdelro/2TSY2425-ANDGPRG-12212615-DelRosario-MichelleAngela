using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;    // How fast planet spins
    [SerializeField] private float orbitSpeed = 30f;       // How fast planet orbits sun
    [SerializeField] private float distanceFromSun;        // Distance from solar system center

    [Header("Rotation Axes")]
    [SerializeField] private bool rotateAroundSelf = true; // Toggle self-rotation
    [SerializeField] private bool rotateAroundSun = true;  // Toggle sun orbit
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private void Start()
    {
        // Calculate initial sun distance if not set
        if (distanceFromSun == 0)
        {
            distanceFromSun = Vector3.Distance(transform.position, Vector3.zero);
        }
    }

    private void Update()
    {
        // Handle planet rotation around its axis
        if (rotateAroundSelf)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        // Handle planet orbit around the sun
        if (rotateAroundSun)
        {
            transform.RotateAround(Vector3.zero,
                                   Vector3.up,
                                   orbitSpeed * Time.deltaTime / Mathf.Sqrt(distanceFromSun));
        }
    }

    // Public methods for external control of rotation
    public void SetRotationSpeed(float speed) { rotationSpeed = speed; }
    public void SetOrbitSpeed(float speed) { orbitSpeed = speed; }
    public void ToggleSelfRotation(bool enable) { rotateAroundSelf = enable; }
    public void ToggleOrbit(bool enable) { rotateAroundSun = enable; }
}
