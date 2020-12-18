using UnityEngine;

public class PhotoCameraMovement : MonoBehaviour
{
    public float sensitivity;

    public float moveSpeed = 10f;
    public float fasterMoveSpeed = 15f;

    private float initalMoveSpeed;

    private Vector2 mouseAxis;

    private void Awake()
    {
        initalMoveSpeed = moveSpeed;
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
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        float rotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity;

        //clamping
        Debug.Log(rotationY);
        rotationY = ClampAngle(rotationY, -90, 90);

        //rotation
        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
