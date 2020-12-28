using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemManager : MonoBehaviour
{
    private UnscaledFixedUpdate updateInputSystem;

    private void Awake()
    {
        updateInputSystem = new UnscaledFixedUpdate(0.02f, UpdateInputSystem);
    }

    private void Update()
    {
        updateInputSystem.Update(Time.unscaledDeltaTime);
    }

    private void UpdateInputSystem() => InputSystem.Update();
}
