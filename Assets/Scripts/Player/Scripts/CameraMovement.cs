using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Handles the camera movement in-game.
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        public CinemachineFollowZoom cinemachineFollowZoom;

        public float movementSpeed;
        public float movementTime;
        public float rotationSpeed = 100;
        public float dragSpeed;
        public float zoomSpeed;

        private Vector3 moveDirection;
        private float rotationDirection;
        private bool isDragging;
        private Vector2 mouseDelta;
        private bool isPanning;
        private float mouseScrollY;

        private float mouseYaw = 0f;
        private float zoomLevel;
        private float maxFOV;
        private float minFOV;

        [Header("World Boundaries")]
        public bool enableWorldBoundaries = true;
        public bool showGizmo = true;
        public float maxX;
        public float minX;
        public float maxZ;
        public float minZ;

        private void Start()
        {
            zoomLevel = cinemachineFollowZoom.m_MaxFOV;
            maxFOV = cinemachineFollowZoom.m_MaxFOV;
            minFOV = cinemachineFollowZoom.m_MinFOV;
        }

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();
            ApplyDrag();
            ApplyZoom();
        }

        public void Move(InputAction.CallbackContext context) => moveDirection = context.ReadValue<Vector3>();

        public void Rotate(InputAction.CallbackContext context) => rotationDirection = context.ReadValue<float>();

        public void DragState(InputAction.CallbackContext context) => isDragging = !isDragging;

        public void Drag(InputAction.CallbackContext context) => mouseDelta = context.ReadValue<Vector2>();

        public void PanRotateState(InputAction.CallbackContext context) => isPanning = !isPanning;

        public void Zoom(InputAction.CallbackContext context) => mouseScrollY = context.ReadValue<float>();

        private void ApplyMovement()
        {
            transform.position = GetMovement(transform.position + (moveDirection * movementSpeed * Time.unscaledDeltaTime));
        }

        private void ApplyRotation()
        {
            if (CamerasManager.Instance.cameraMode.Equals(CameraMode.Normal))
            {
                transform.rotation *= Quaternion.Euler(0, rotationSpeed * rotationDirection / 200f, 0);
            }
        }

        private void ApplyDrag()
        {
            if (isDragging)
            {
                float speed = dragSpeed * Time.unscaledDeltaTime;
                transform.position = GetMovement(transform.position - (mouseDelta.x * speed * transform.right + mouseDelta.y * speed * transform.forward));
            }

            if (isPanning && CamerasManager.Instance.cameraMode.Equals(CameraMode.Normal))
            {
                mouseYaw += mouseDelta.x * 8f;
                transform.rotation = Quaternion.Euler(0, mouseYaw / 200f, 0);
            }
        }

        private void ApplyZoom()
        {
            if (CamerasManager.Instance.cameraMode.Equals(CameraMode.Normal))
            {
                if (mouseScrollY > 0 && zoomLevel >= minFOV)
                {
                    // scroll up
                    zoomLevel -= Time.unscaledDeltaTime * zoomSpeed * 10;
                }
                else if (mouseScrollY < 0 && zoomLevel <= maxFOV)
                {
                    // scroll down
                    zoomLevel += Time.unscaledDeltaTime * zoomSpeed * 10;
                }

                cinemachineFollowZoom.m_MaxFOV = Mathf.Lerp(cinemachineFollowZoom.m_MaxFOV, zoomLevel, Time.unscaledDeltaTime * zoomSpeed);
            }
        }

        private Vector3 GetMovement(Vector3 newPos)
        {
            if (enableWorldBoundaries)
                return new Vector3(Mathf.Clamp(newPos.x, minX, maxX), transform.position.y, Mathf.Clamp(newPos.z, minZ, maxZ));
            else
                return new Vector3(newPos.x, transform.position.y, newPos.z);
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
}
