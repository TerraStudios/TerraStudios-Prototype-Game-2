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

using TerrainGeneration;
using UnityEngine;

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
