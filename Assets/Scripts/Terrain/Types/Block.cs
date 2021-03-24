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
using TerrainGeneration;

namespace TerrainTypes
{
    [Serializable]
    public class Block
    {
        public readonly byte value;
        public readonly VoxelType type;

        public Block(byte value, VoxelType type)
        {
            this.value = value;
            this.type = type;
        }
    }
}
