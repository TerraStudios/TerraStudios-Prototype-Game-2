using BuildingManagement;
using UnityEngine;

namespace BuildingModules
{
    /// <summary>
    /// This class handles the conveyor behaviour and is placed on each conveyor.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Conveyor : MonoBehaviour, IConveyorBase
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
}
