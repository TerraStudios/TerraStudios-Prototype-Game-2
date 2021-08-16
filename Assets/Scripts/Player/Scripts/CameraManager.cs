﻿//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
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
        public GameObject dynamicPointers;

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
            // apply pos and rot from the last camera pos and rot
            if (cameraMode != CameraMode.Topdown)
                transform.position = lastPosition;
            transform.rotation = lastRotation;

            normalCamera.SetActive(true);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            cameraMode = CameraMode.Normal;
            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
            dynamicPointers.SetActive(true);
        }

        public void SwitchToTopdownCamera()
        {
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

            cameraMode = CameraMode.Topdown;
            cameraMovement.enabled = true;
            photoCameraMovement.enabled = false;
            dynamicPointers.SetActive(false);
        }

        public void SwitchToFreecam()
        {
            // save last position and rotation so we can restore them when we return to vcam 1 or 2
            if (cameraMode == CameraMode.Normal)
            {
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }

            normalCamera.SetActive(false);
            topDownCamera.SetActive(false);
            freeCamera.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            cameraMode = CameraMode.Freecam;
            cameraMovement.enabled = false;
            photoCameraMovement.enabled = true;
            dynamicPointers.SetActive(false);
        }
    }
}
