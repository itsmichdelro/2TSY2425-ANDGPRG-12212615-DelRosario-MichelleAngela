using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject[] bulletPrefabs;  // Different bullet types
    [SerializeField] private Transform[] nozzlePoints;    // Reference to nozzle positions

    private int currentNozzle = 0;
    private float[] fireRates = { 0.4f, 0.3f, 0.2f, 0.1f };
    private float nextFireTime = 0f;

    void Update()
    {
        HandleMovement();
        HandleNozzleSwitch();
        HandleShooting();
    }

    void HandleMovement()
    {
        // Get input axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Apply movement
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Only rotate if there's movement input
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
            // Special handling for key 2 to fire from two nozzles
            if (currentNozzle == 1 && nozzlePoints.Length >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Transform currentNozzleTransform = nozzlePoints[i];
                    GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                   currentNozzleTransform.position,
                                                   currentNozzleTransform.rotation);
                }
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
            }
            // Special handling for key 4 to fire from 4 nozzles
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
            }
            // Original single nozzle firing logic for other keys
            else
            {
                Transform currentNozzleTransform = nozzlePoints[currentNozzle];
                GameObject bullet = Instantiate(bulletPrefabs[currentNozzle],
                                                currentNozzleTransform.position,
                                                currentNozzleTransform.rotation);
                nextFireTime = Time.time + fireRates[currentNozzle];
            }
        }
    }
}