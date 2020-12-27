using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the photo camera movement in-game (freecam).
/// </summary>
public class PhotoCameraMovement : MonoBehaviour
{
    public float sensitivity;

    public float moveSpeed = 10f;
    public float fasterMoveSpeed = 15f;

    private Vector3 moveDirection;
    private Vector2 mouseDelta;
    private float upAndDownDirection;
    private bool isShiftPressed;

    private float initalMoveSpeed;

    private Vector3 rotation;

    private void Awake()
    {
        initalMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        // Set initial angles
        rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        ApplyMovement();
        ApplyUpAndDownMovement();
        ApplyRotation(mouseDelta);
    }

    public void Move(InputAction.CallbackContext context) => moveDirection = context.ReadValue<Vector3>();

    public void Look(InputAction.CallbackContext context) => mouseDelta = context.ReadValue<Vector2>();

    public void MoveUpAndDown(InputAction.CallbackContext context) => upAndDownDirection = context.ReadValue<float>();

    public void ShiftSpeedState(InputAction.CallbackContext context)
    {
        isShiftPressed = !isShiftPressed;

        if (isShiftPressed)
        {
            moveSpeed = fasterMoveSpeed;
        }
        else
        {
            moveSpeed = initalMoveSpeed;
            if (initalMoveSpeed > moveSpeed)
            {
                moveSpeed = initalMoveSpeed;
            }
        }
    }

    public void ApplyMovement()
    {
        transform.position += transform.TransformDirection(moveDirection) * moveSpeed * Time.unscaledDeltaTime;
    }

    private void ApplyRotation(Vector2 direction)
    {
        float speed = sensitivity * Time.unscaledDeltaTime;

        // Add current mouse x axis delta
        rotation.y += direction.x * speed;

        // Clamp y axis to not roll
        rotation.x = Mathf.Clamp(rotation.x - direction.y * speed, -90, 90);

        // Set rotation using quaternions
        // Using localEulerAngles proved to be inadequate as the game was experiencing what is known as the gimbal lock when looking straight up or down
        //transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
        transform.localEulerAngles = rotation;
    }

    private void ApplyUpAndDownMovement()
    {
        transform.position += transform.up * upAndDownDirection * moveSpeed * Time.unscaledDeltaTime;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        //if (angle < -360F)
        //    angle += 360F;
        //if (angle > 360F)
        //    angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
