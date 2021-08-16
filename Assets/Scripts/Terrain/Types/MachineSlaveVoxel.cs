﻿//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using BuildingModules;
using TerrainGeneration;

namespace TerrainTypes
{
    class MachineSlaveVoxel : Voxel
    {
        public readonly Building controller;

        public MachineSlaveVoxel(VoxelType solid, Building controller) : base(1, solid)
        {
            this.controller = controller;
        }
    }
}
