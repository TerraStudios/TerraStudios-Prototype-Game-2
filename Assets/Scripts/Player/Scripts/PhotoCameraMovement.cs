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
        /*mouseAxis.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.unscaledDeltaTime;
        mouseAxis.y += Input.GetAxis("Mouse X") * sensitivity * Time.unscaledDeltaTime;
        mouseAxis.x = Mathf.Clamp(mouseAxis.x, minY, maxY);

        transform.rotation = Quaternion.Euler(mouseAxis.x, mouseAxis.y, 0);*/

        mouseAxis.x += -Input.GetAxis("Mouse Y") * sensitivity;
        mouseAxis.y += Input.GetAxis("Mouse X") * sensitivity;

        //clamping
        mouseAxis.x = Mathf.Clamp(mouseAxis.x, -90, 90);

        //rotation
        transform.localRotation = Quaternion.Euler(mouseAxis.x, mouseAxis.y, 0);
    }
}
