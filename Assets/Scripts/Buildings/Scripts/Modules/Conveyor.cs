using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position += -rb.transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }
}
