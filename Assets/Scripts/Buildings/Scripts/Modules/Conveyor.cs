﻿using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public ModuleConnector mc;
    public float speed;

    public List<Collider> itemsOnTop;

    private void OnTriggerEnter(Collider other)
    {
        if (!itemsOnTop.Contains(other) && !other.gameObject.layer.Equals(11))
            return;

        itemsOnTop.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.layer.Equals(11) && itemsOnTop.Contains(other))
            return;

        itemsOnTop.Remove(other);
    }

    void FixedUpdate()
    {
        if (mc.Building.isSetUp)
        {
            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                Collider other = itemsOnTop[i];

                if (!other)
                    continue;

                Rigidbody otherRB = other.attachedRigidbody;
                //otherRB.velocity = transform.forward * speed;

                float conveyorVelocity = speed * Time.deltaTime;
                otherRB.position = Vector3.MoveTowards(otherRB.position, otherRB.position + transform.forward, conveyorVelocity);
            }
        }
    }

    // Old belt movement code
    /*void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position += -rb.transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }*/
}
