//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using BuildingManagement;
using BuildingModules;
using TerrainGeneration;
using System.Linq;

namespace TerrainTypes
{
    [Serializable]
    class MachineSlaveVoxel : Voxel
    {
        public readonly int keylistID;
        public readonly int valueListID;

        public MachineSlaveVoxel(VoxelType solid, int buildingListID, int buildingID) : base(1, solid)
        {
            keylistID = buildingListID;
            valueListID = buildingID;
        }

        public Building GetBuilding()
        {
            return BuildingSystem.PlacedBuildings.Values.ElementAt(keylistID)[valueListID].Key;
        }
    }
}
