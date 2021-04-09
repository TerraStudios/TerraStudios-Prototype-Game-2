//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using TerrainGeneration;
using UnityEngine;

namespace TerrainGeneration
{
    /// <summary>
    /// Modifies properties/fields of TerrainGenerator to make world generation fit better with the gameplay.
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        public TerrainGenerator terrainGen;
        public Transform vCam;
        public LayerMask voxelLayer;

        [Header("Chunk Range Customization")]
        public bool enableChunkRangeOverride = true;
        public float minYVoxelDistance;
        public float maxYVoxelDistance;

        public int minChunkRange;
        public int maxChunkRange;

        [Header("Debug")]
        public float currentYVoxelDistance;
        public float currentChunkRange;
        public int appliedChunkRange;

        private void Update()
        {
            if (enableChunkRangeOverride)
            {
                float distance = GetCameraTerrainDistance();

                if (distance == 0) // Make sure to ignore the middle section where we're neither above or below the terrain (inside of it).
                    return;

                currentYVoxelDistance = Mathf.Clamp(distance, minYVoxelDistance, maxYVoxelDistance);
                currentChunkRange = Mathf.Lerp(minChunkRange, maxChunkRange, currentYVoxelDistance / maxYVoxelDistance);
                appliedChunkRange = Mathf.FloorToInt(currentChunkRange);
                int previousRange = terrainGen.chunkRange;
                terrainGen.chunkRange = appliedChunkRange;

                if (previousRange != terrainGen.chunkRange) // Only force reload when the value is actually changed.
                    terrainGen.forceChunkCheck = true;
            }
        }

        /// <summary>
        /// Gets distance from vCam 1 to the voxel terrain.
        /// Tries both up and down direction to ensure the terrain is found even when beneath it.
        /// </summary>
        /// <returns>The Distance</returns>
        private float GetCameraTerrainDistance()
        {
            if (Physics.Raycast(vCam.position, Vector3.down, out RaycastHit hitDown, Mathf.Infinity, voxelLayer))
                return hitDown.distance;
            else if (Physics.Raycast(vCam.position, Vector3.up, out RaycastHit hitUp, Mathf.Infinity, voxelLayer))
                return hitUp.distance;
            else
                return 0;
        }
    }
}
