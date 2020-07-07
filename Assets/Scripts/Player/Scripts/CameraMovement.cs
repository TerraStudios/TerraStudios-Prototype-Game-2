using Cinemachine;
using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform CameraGO;
    public CinemachineFollowZoom CinemachineFollowZoom;

    public float movementSpeed;
    public float movementTime;
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
            transform.position = GetMovement(transform.position + (transform.forward * movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = GetMovement(transform.position + (-transform.forward * movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = GetMovement(transform.position + (transform.right * movementSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = GetMovement(transform.position + (-transform.right * movementSpeed * Time.deltaTime));
        }

        // Q and E 90 degrees rotation
        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(0, 90, 0);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(0, -90, 0);
        }
        */
    }

    private void HandleMouseMovement()
    {
        // Mouse Drag movement

        if (Input.GetMouseButton(1))
        {
            float speed = dragSpeed * Time.deltaTime;
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
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * 10;

        }
        else if (d < 0f && zoomLevel <= maxFOV)
        {
            // scroll down
            zoomLevel += -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * 10;
        }
    }

    private Vector3 GetMovement(Vector3 newPos)
    {
        return new Vector3(Mathf.Clamp(newPos.x, minX, maxX), transform.position.y, Mathf.Clamp(newPos.z, minZ, maxZ));
    }

    private void ApplyCameraFOV()
    {
        CinemachineFollowZoom.m_MaxFOV = Mathf.Lerp(CinemachineFollowZoom.m_MaxFOV, zoomLevel, Time.deltaTime * zoomSpeed);
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
