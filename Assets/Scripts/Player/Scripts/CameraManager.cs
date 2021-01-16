//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// The types of Camera modes.
    /// Each camera modes correspond to a Cinemachine camera.
    /// </summary>
    public enum CameraMode
    {
        Normal,
        Topdown,
        Freecam
    }

    /// <summary>
    /// Manages all Cinemachine cameras and camera modes.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        public CameraMode cameraMode;
        public PlayerInput inputManager;

        public GameObject normalCamera;
        public GameObject topDownCamera;
        public GameObject freeCamera;

        public CameraMovement cameraMovement;
        public PhotoCameraMovement photoCameraMovement;

        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private void Awake()
        {
            Instance = this;

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        public void SwitchTopDownCamera() 
        {
            if (!cameraMode.Equals(CameraMode.Topdown))
            {
                SwitchToTopdownCamera();
            }
            else
                SwitchToNormalCamera();
        }

        public void SwitchFreecam()
        {
            if (!cameraMode.Equals(CameraMode.Freecam))
                SwitchToFreecam();
            else
                SwitchToNormalCamera();
        }

        public void SwitchToNormalCamera()
        {
            cameraMode = CameraMode.Normal;

            // apply pos and rot from the last camera pos and rot
            transform.position = lastPosition;
            transform.rotation = lastRotation;

            normalCamera.SetActive(true);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
        }

        public void SwitchToTopdownCamera()
        {
            cameraMode = CameraMode.Topdown;

            // set lastRot
            // apply pos and rot from the last camera pos and rot. the rot is 0 because the camera has to look straight down
            lastRotation = transform.rotation;
            transform.position = lastPosition;
            transform.rotation = Quaternion.identity;

            normalCamera.SetActive(false);
            topDownCamera.SetActive(true);
            freeCamera.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
        }

        public void SwitchToFreecam()
        {
            // save last position and rotation so we can restore them when we return to vcam 1 or 2
            if (cameraMode == CameraMode.Normal)
            {
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }

            cameraMode = CameraMode.Freecam;

            normalCamera.SetActive(false);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            cameraMovement.enabled = false;
            photoCameraMovement.enabled = true;
        }
    }
}
