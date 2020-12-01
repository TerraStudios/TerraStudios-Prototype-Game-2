using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Conveyor : MonoBehaviour, ConveyorBase
{
    public Rigidbody rb;
    public float speed = 1;


    public void UpdateConveyor()
    {

        Vector3 pos = rb.position;
        rb.position += -rb.transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }

}
