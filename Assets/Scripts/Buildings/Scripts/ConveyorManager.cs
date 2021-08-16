//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System.Collections.Generic;
using UnityEngine;

namespace BuildingManagement
{
    public interface IConveyorBase
    {
        void UpdateConveyor();
        void LateUpdateConveyor();
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

        private void Update()
        {
            conveyors.ForEach(conveyor => conveyor.UpdateConveyor());
        }

        private void LateUpdate()
        {
            conveyors.ForEach(conveyor => conveyor.LateUpdateConveyor());
        }
    }
}
