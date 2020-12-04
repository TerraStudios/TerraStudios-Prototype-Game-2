using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all placed conveyors.
/// </summary>
public class ConveyorManager : MonoBehaviour
{
    public static ConveyorManager instance;

    public List<ConveyorBase> conveyors = new List<ConveyorBase>();

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        conveyors.ForEach(conveyor => conveyor.UpdateConveyor());
    }
}
