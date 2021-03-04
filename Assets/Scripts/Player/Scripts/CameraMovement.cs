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
        public CinemachineVirtualCamera cinemachineVCam;
        private CinemachineTransposer cinemachineTransposer;

        public float movementSpeed;
        public float upAndDownSpeed;
        public float rotationSpeed = 100;
        public float dragSpeed;
        public float topDownDragMultiplier;
        public float panSpeed = 0.5f;

        [Header("Zooming")]
        public float zoomMultiplier;
        public float zoomSmoothnessMultiplier;
        public float minFollowOffsetY;
        //public float followProgress;
        public float followProgressApplied;
        public float maxFollowOffsetY;
        public AnimationCurve followOffsetCurve;

        public float minFOVZoom;
        //public float fovZoomLevel;
        public float fovZoomLevelApplied;
        public float maxFOVZoom;
        public AnimationCurve fovZoomCurve;

        [Header("Zooming Experimental - You most likely shouldn't need to touch this")]
        public float minZoomProgress;
        public float zoomProgress = 1;
        public float maxZoomProgress = 1;

        private Vector3 moveDirection;
        private float rotationDirection;
        private float upAndDownDirection;
        private bool isDragging;
        private Vector2 mouseDelta;
        private bool isPanning;
        private float mouseScrollY;

        private float mouseYawXForPanning;

        [Header("World Boundaries")]
        public bool enableWorldBoundaries = true;
        public bool showGizmo = true;

        public Vector3 boundariesMin;
        public Vector3 boundariesMax;

        private void Start()
        {
            mouseYawXForPanning = transform.rotation.eulerAngles.x;
            //fovZoomLevel = cinemachineVCam.m_Lens.FieldOfView;
            cinemachineTransposer = cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>();
            //followProgress = cinemachineTransposer.m_FollowOffset.y;
        }

        private void Update()
        {
            ApplyMovement();
            ApplyRotation();
            ApplyUpAndDownMovement();
            ApplyDrag();
            ApplyZoom();
        }

        public void Move(InputAction.CallbackContext context) => moveDirection = context.ReadValue<Vector3>();

        public void Rotate(InputAction.CallbackContext context) => rotationDirection = context.ReadValue<float>();

        public void UpAndDown(InputAction.CallbackContext context) => upAndDownDirection = context.ReadValue<float>();

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
                transform.rotation *= Quaternion.Euler(0, rotationSpeed * rotationDirection / 200f * Time.unscaledDeltaTime, 0);
            }
        }

        private void ApplyUpAndDownMovement()
        {
            if (CameraManager.Instance.cameraMode.Equals(CameraMode.Normal))
            {
                transform.position = GetMovement(transform.position + (transform.TransformDirection(Vector3.up * upAndDownDirection) * upAndDownSpeed * Time.unscaledDeltaTime));
            }
        }

        private void ApplyDrag()
        {
            if (isDragging)
            {
                float speed = dragSpeed * Time.unscaledDeltaTime;

                if (CameraManager.Instance.cameraMode.Equals(CameraMode.Topdown))
                    speed *= topDownDragMultiplier;

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
                if (mouseScrollY > 0) // Scroll up
                {
                    if (zoomProgress >= minZoomProgress)
                        zoomProgress -= Time.unscaledDeltaTime * zoomMultiplier;
                }

                else if (mouseScrollY < 0) // Scroll down
                {
                    if (zoomProgress <= maxZoomProgress)
                        zoomProgress += Time.unscaledDeltaTime * zoomMultiplier;
                }

                zoomProgress = Mathf.Clamp(zoomProgress, minZoomProgress, maxZoomProgress);

                // Camera FOV
                //fovZoomLevel = Mathf.Lerp(minFOVZoom, maxFOVZoom, zoomProgress);
                fovZoomLevelApplied = Mathf.Lerp(minFOVZoom, maxFOVZoom, fovZoomCurve.Evaluate(zoomProgress));
                cinemachineVCam.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVCam.m_Lens.FieldOfView, fovZoomLevelApplied, Time.unscaledDeltaTime * zoomSmoothnessMultiplier);

                // Camera Offset
                //followProgress = Mathf.Lerp(minFollowOffsetY, maxFollowOffsetY, zoomProgress);
                followProgressApplied = Mathf.Lerp(minFollowOffsetY, maxFollowOffsetY, followOffsetCurve.Evaluate(zoomProgress));
                cinemachineTransposer.m_FollowOffset.y = Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y, followProgressApplied, Time.unscaledDeltaTime * zoomSmoothnessMultiplier);
            }
        }

        private Vector3 GetMovement(Vector3 newPos)
        {
            if (enableWorldBoundaries)
                return new Vector3(Mathf.Clamp(newPos.x, boundariesMin.x, boundariesMax.x), Mathf.Clamp(newPos.y, boundariesMin.y, boundariesMax.y), Mathf.Clamp(newPos.z, boundariesMin.z, boundariesMax.z));
            else
                return new Vector3(newPos.x, transform.position.y, newPos.z);
        }

        private void OnDrawGizmos()
        {
            if (showGizmo)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(new Vector3((boundariesMax.x + boundariesMin.x) / 2, (boundariesMax.y + boundariesMin.y) / 2, (boundariesMax.z + boundariesMin.z) / 2), new Vector3(Mathf.Abs(boundariesMax.x) + Mathf.Abs(boundariesMin.x), Mathf.Abs(boundariesMax.y) + Mathf.Abs(boundariesMin.y), Mathf.Abs(boundariesMax.z) + Mathf.Abs(boundariesMin.z)));
            }
        }
    }
}
