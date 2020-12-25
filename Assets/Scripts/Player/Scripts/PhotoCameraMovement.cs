using UnityEngine;

/// <summary>
/// Handles the photo camera movement in-game (freecam).
/// </summary>
public class PhotoCameraMovement : MonoBehaviour
{
    public float sensitivity;

    public float moveSpeed = 10f;
    public float fasterMoveSpeed = 15f;

    private float initalMoveSpeed;

    float horizontal = 0;
    float vertical = 0;

    private void Awake()
    {
        initalMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        // Set initial angles
        horizontal = transform.rotation.eulerAngles.y;
        vertical = transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
    }

    public void ApplyMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
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

        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space))
        {
            transform.position += transform.up * moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * moveSpeed * Time.unscaledDeltaTime;
        }


        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        transform.position += transform.forward * Z * moveSpeed * Time.unscaledDeltaTime;
        transform.position += transform.right * X * moveSpeed * Time.unscaledDeltaTime;
    }



    public void ApplyRotation()
    {
        // Add current mouse x axis delta
        horizontal += Input.GetAxis("Mouse X") * sensitivity;

        // Subtract current mouse y axis delta, as y is inverted
        vertical -= Input.GetAxis("Mouse Y") * sensitivity;

        // Clamp y axis to not roll
        vertical = Mathf.Clamp(vertical, -90, 90);

        // Set rotation using quaternions
        // Using localEulerAngles proved to be inadequate as the game was experiencing what is known as the gimbal lock when looking straight up or down
        transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
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
