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

        private void Awake()
        {
            Instance = this;
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

            normalCamera.SetActive(true);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(false);

            normalCamera.transform.rotation = cameraMovement.cameraRotation;

            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
        }

        public void SwitchToTopdownCamera()
        {
            cameraMode = CameraMode.Topdown;

            normalCamera.SetActive(false);
            topDownCamera.SetActive(true);
            freeCamera.SetActive(false);

            cameraMovement.cameraRotation = normalCamera.transform.rotation;
            topDownCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
        }

        public void SwitchToFreecam()
        {
            cameraMode = CameraMode.Freecam;

            normalCamera.SetActive(false);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(true);

            cameraMovement.enabled = false;
            photoCameraMovement.enabled = true;
        }
    }
}
