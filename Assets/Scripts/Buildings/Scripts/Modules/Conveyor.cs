using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Conveyor : MonoBehaviour
{
    public ModuleConnector mc;
    public float speed = 1;
    public Rigidbody rb;

    public List<Collider> itemsOnTop;

    // Old belt movement code
    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position += -rb.transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }
}
