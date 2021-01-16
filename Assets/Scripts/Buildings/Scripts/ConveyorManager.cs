﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using UnityEngine;

namespace BuildingManagement
{
    public interface IConveyorBase
    {
        void UpdateConveyor();
    }

    /// <summary>
    /// Manages all placed conveyors.
    /// </summary>
    public class ConveyorManager : MonoBehaviour
    {
        public static ConveyorManager Instance;

        public List<IConveyorBase> conveyors = new List<IConveyorBase>();

        private void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            conveyors.ForEach(conveyor => conveyor.UpdateConveyor());
        }
    }
}
