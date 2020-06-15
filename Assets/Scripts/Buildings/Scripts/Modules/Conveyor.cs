using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.layer.Equals(11))
            return;

        Rigidbody otherRB = other.attachedRigidbody;
        //otherRB.velocity = transform.forward * speed;

        float conveyorVelocity = speed * Time.deltaTime;
        otherRB.position = Vector3.MoveTowards(otherRB.position, otherRB.position + transform.forward, conveyorVelocity);
    }

    /*void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position += -rb.transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }*/
}
