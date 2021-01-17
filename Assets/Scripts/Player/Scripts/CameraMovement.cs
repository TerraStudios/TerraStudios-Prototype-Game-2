//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

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
        public float panSpeed = 0.5f;
        public float zoomSpeed;

        private Vector3 moveDirection;
        private float rotationDirection;
        private bool isDragging;
        private Vector2 mouseDelta;
        private bool isPanning;
        private float mouseScrollY;

        private float mouseYawXForPanning;
        private float zoomLevel;
        private float maxFOV;
        private float minFOV;

        [Header("World Boundaries")]
        public bool enableWorldBoundaries = true;
        public bool showGizmo = true;
        public float maxX;
        public float minX;
        public float maxY;
        public float minY;
        public float maxZ;
        public float minZ;

        private void Start()
        {
            mouseYawXForPanning = transform.rotation.eulerAngles.x;
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

        public void DragState(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isDragging = !isDragging;
            }
        }

        public void Drag(InputAction.CallbackContext context) => mouseDelta = context.ReadValue<Vector2>();

        public void PanRotateState(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isPanning = !isPanning;
                mouseYawXForPanning = transform.rotation.eulerAngles.y;
            }
        }

        public void Zoom(InputAction.CallbackContext context) => mouseScrollY = context.ReadValue<float>();

        private void ApplyMovement()
        {
            transform.position = GetMovement(transform.position + (transform.TransformDirection(moveDirection) * movementSpeed * Time.unscaledDeltaTime));
        }

        private void ApplyRotation()
        {
            if (CameraManager.Instance.cameraMode.Equals(CameraMode.Normal))
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

            if (isPanning && CameraManager.Instance.cameraMode.Equals(CameraMode.Normal))
            {
                mouseYawXForPanning += mouseDelta.x * panSpeed * Time.unscaledDeltaTime;
                transform.rotation = Quaternion.Euler(0, mouseYawXForPanning, 0);
            }
        }

        private void ApplyZoom()
        {
            if (CameraManager.Instance.cameraMode.Equals(CameraMode.Normal))
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
                return new Vector3(Mathf.Clamp(newPos.x, minX, maxX), Mathf.Clamp(newPos.y, minY, maxY), Mathf.Clamp(newPos.z, minZ, maxZ));
            else
                return new Vector3(newPos.x, transform.position.y, newPos.z);
        }

        private void OnDrawGizmos()
        {
            if (showGizmo)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2), new Vector3(Mathf.Abs(maxX) + Mathf.Abs(minX), Mathf.Abs(maxY) + Mathf.Abs(minY), Mathf.Abs(maxZ) + Mathf.Abs(minZ)));
            }
        }
    }
}
