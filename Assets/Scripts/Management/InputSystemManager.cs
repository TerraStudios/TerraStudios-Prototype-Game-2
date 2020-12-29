using UnityEngine;
using UnityEngine.InputSystem;
using CoreManagement;

namespace CoreManagement
{
    public class InputSystemManager : MonoBehaviour
    {
        public static InputSystemManager Instance;
        public PlayerInput playerInput;
        private UnscaledFixedUpdate updateInputSystem;

        private void Awake()
        {
            Instance = this;
            updateInputSystem = new UnscaledFixedUpdate(0.02f, UpdateInputSystem);
        }

        private void Update()
        {
            updateInputSystem.Update(Time.unscaledDeltaTime);
        }

        private void UpdateInputSystem() => InputSystem.Update();

        public void SwitchToGameplay() => playerInput.SwitchCurrentActionMap("Gameplay");

        public void SwitchToPauseMenu() => playerInput.SwitchCurrentActionMap("Pause Menu");
    }
}
