using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Variables")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float targetFov;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoom = 10;
    [SerializeField] private float maxZoom = 100;

    [Header("Boundaries")]
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private float topLimit;
    [SerializeField] private float bottomLimit;

    // Update is called once per frame
    void LateUpdate()
    {
        MovementInput();
        HandleCameraZoom();
        HandleCameraEdgeScrolling();
    }

    void MovementInput()
    {
        Vector3 inputDir = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputDir.z += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            inputDir.z -= movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputDir.x -= movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputDir.x += movementSpeed * Time.deltaTime;
        }

        inputDir.x = Mathf.Clamp(inputDir.x, leftLimit, rightLimit);
        inputDir.z = Mathf.Clamp(inputDir.z, topLimit, bottomLimit);

        transform.position = inputDir;
    }

    void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)//zoom in
        {
            targetFov -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)//zoom out
        {
            targetFov += 5;
        }

        targetFov = Mathf.Clamp(targetFov, minZoom, maxZoom);
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFov, Time.deltaTime * zoomSpeed);

    }

    void HandleCameraEdgeScrolling()
    {
        Vector3 inputDir = transform.position;
        int edgeScrollSize = 20;

        if (Input.mousePosition.y > Screen.height - edgeScrollSize)//up
        {
            inputDir.z += movementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y < edgeScrollSize)//down
        {
            inputDir.z -= movementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x < edgeScrollSize)//left
        {
            inputDir.x -= movementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x > Screen.width - edgeScrollSize)//right
        {
            inputDir.x += movementSpeed * Time.deltaTime;
        }

        inputDir.x = Mathf.Clamp(inputDir.x, leftLimit, rightLimit);
        inputDir.z = Mathf.Clamp(inputDir.z, topLimit, bottomLimit);

        transform.position = inputDir;
    }
}