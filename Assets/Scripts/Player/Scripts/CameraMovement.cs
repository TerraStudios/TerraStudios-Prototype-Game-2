using Cinemachine;
using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform CameraGO1;
    public Transform CameraGO2;

    public CinemachineFollowZoom CinemachineFollowZoom;

    public float movementSpeed;
    public float movementTime;
    public float rotationSpeed = 100;
    public float dragSpeed;
    public float zoomSpeed;

    private float mouseYaw = 0f;
    private float zoomLevel;
    private float maxFOV;
    private float minFOV;

    [Header("World Boundries")]
    public bool showGizmo = true;
    public float maxX;
    public float minX;
    public float maxZ;
    public float minZ;

    // Saved camera rotation when switching to the secondary camera (with Z)
    // Used to restore rotation after switching back
    private Quaternion cameraRotation;

    private void Start()
    {
        zoomLevel = CinemachineFollowZoom.m_MaxFOV;
        maxFOV = CinemachineFollowZoom.m_MaxFOV;
        minFOV = CinemachineFollowZoom.m_MinFOV;
    }

    private void Update()
    {
        HandleKeyboardMovement();
        HandleMouseMovement();
        ApplyCameraFOV();
    }

    private void HandleKeyboardMovement()
    {
        // WASD + Arrows Movement

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = GetMovement(transform.position + (transform.forward * movementSpeed * Time.unscaledDeltaTime));
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = GetMovement(transform.position + (-transform.forward * movementSpeed * Time.unscaledDeltaTime));
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = GetMovement(transform.position + (transform.right * movementSpeed * Time.unscaledDeltaTime));
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = GetMovement(transform.position + (-transform.right * movementSpeed * Time.unscaledDeltaTime));
        }

        // Only allow the normal camera to rotate
        if (CameraGO1.gameObject.activeInHierarchy)
        {

            // Q and E 90 degrees rotation
            if (Input.GetKey(KeyCode.Q))
            {
                transform.rotation *= Quaternion.Euler(0, rotationSpeed / 200f, 0);
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.rotation *= Quaternion.Euler(0, -rotationSpeed / 200f, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (CameraGO1.gameObject.activeInHierarchy)
            {
                cameraRotation = CameraGO1.rotation;
                CameraGO1.gameObject.SetActive(false);
                CameraGO2.gameObject.SetActive(true);
                CameraGO2.rotation = Quaternion.Euler(90, 0, 0);
            } else
            {
                CameraGO1.gameObject.SetActive(true);
                CameraGO1.rotation = cameraRotation;
                CameraGO2.gameObject.SetActive(false);
            }
        }
        
    }

    private void HandleMouseMovement()
    {
        // Mouse Drag movement

        if (Input.GetMouseButton(1))
        {
            float speed = dragSpeed * Time.unscaledDeltaTime;
            transform.position = GetMovement(transform.position - (Input.GetAxis("Mouse X") * speed * transform.right + Input.GetAxis("Mouse Y") * speed * transform.forward));
        }

        // Middle click rotation

        if (Input.GetKey(KeyCode.Mouse2))
        {
            mouseYaw += 2f * Input.GetAxis("Mouse X");
            transform.rotation = Quaternion.Euler(0, mouseYaw, 0);
        }

        float d = Input.GetAxis("Mouse ScrollWheel");

        if (d > 0f && zoomLevel >= minFOV)
        {
            // scroll up
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * zoomSpeed * 10;

        }
        else if (d < 0f && zoomLevel <= maxFOV)
        {
            // scroll down
            zoomLevel += -Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * zoomSpeed * 10;
        }
    }

    private Vector3 GetMovement(Vector3 newPos)
    {
        return new Vector3(Mathf.Clamp(newPos.x, minX, maxX), transform.position.y, Mathf.Clamp(newPos.z, minZ, maxZ));
    }

    private void ApplyCameraFOV()
    {
        CinemachineFollowZoom.m_MaxFOV = Mathf.Lerp(CinemachineFollowZoom.m_MaxFOV, zoomLevel, Time.unscaledDeltaTime * zoomSpeed);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3((maxX + minX) / 2, 0, (maxZ + minZ) / 2), new Vector3(Mathf.Abs(maxX) + Mathf.Abs(minX), 100, Mathf.Abs(maxZ) + Mathf.Abs(minZ)));
        }
    }
}
