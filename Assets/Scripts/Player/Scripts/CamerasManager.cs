using UnityEngine;

namespace Player
{
    public enum CameraMode
    {
        Normal,
        Topdown,
        Freecam
    }

    public class CamerasManager : MonoBehaviour
    {
        public static CamerasManager Instance;

        public CameraMode cameraMode;

        public GameObject normalCamera;
        public GameObject topDownCamera;
        public GameObject freeCamera;

        public CameraMovement cameraMovement;
        public PhotoCameraMovement photoCameraMovement;

        public Vector3 lastPosition;
        public Quaternion lastRotation;

        private void Awake()
        {
            Instance = this;

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!cameraMode.Equals(CameraMode.Topdown))
                {
                    SwitchToTopdownCamera();
                }
                else
                    SwitchToNormalCamera();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!cameraMode.Equals(CameraMode.Freecam))
                    SwitchToFreecam();
                else
                    SwitchToNormalCamera();
            }
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
