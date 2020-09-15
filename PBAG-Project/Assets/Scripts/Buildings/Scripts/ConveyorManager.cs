using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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