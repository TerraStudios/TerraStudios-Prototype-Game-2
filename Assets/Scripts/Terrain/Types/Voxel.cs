//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;

namespace TerrainGeneration
{
    [Serializable]
    public class Voxel
    {
        public readonly byte value;
        public readonly VoxelType type;

        public Voxel(byte value, VoxelType type)
        {
            this.value = value;
            this.type = type;
        }
    }
}
