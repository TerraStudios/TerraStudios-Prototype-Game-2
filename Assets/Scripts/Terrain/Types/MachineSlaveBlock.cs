//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingModules;
using TerrainGeneration;

namespace TerrainTypes
{
    class MachineSlaveBlock : Block
    {
        public readonly Building controller;

        public MachineSlaveBlock(VoxelType solid, Building controller) : base(1, solid)
        {
            this.controller = controller;
        }
    }
}
