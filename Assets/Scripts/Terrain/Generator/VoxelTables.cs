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

using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration
{
    /// <summary>
    /// Stores constant data about each voxel (cube)
    /// </summary>
    public static class VoxelTables
    {
        /// <summary>
        /// Lookup table for the vertices in a cube
        /// </summary>
        public static readonly float3[] VoxelVerts = {

        new float3(0, 0, 0),
        new float3(1, 0, 0),
        new float3(1, 1, 0),
        new float3(0, 1, 0),
        new float3(0, 0, 1),
        new float3(1, 0, 1),
        new float3(1, 1, 1),
        new float3(0, 1, 1),


    };

        /// <summary>
        /// Lookup table for the faces of a cube
        /// </summary>
        public static readonly int3[] Faces =
        {
        new int3(0, 0, -1),
        new int3(0, 0, 1),
        new int3(0, 1, 0),
        new int3(0, -1, 0),
        new int3(-1, 0, 0),
        new int3(1, 0, 0)
        };

        /// <summary>
        /// Lookup table for the triangles of a cube
        /// </summary>
        public static readonly int[] VoxelTris = {

        0, 3, 1, 2, // Back Face
		5, 6, 4, 7, // Front Face
		3, 7, 2, 6, // Top Face
		1, 5, 0, 4, // Bottom Face
		4, 7, 0, 3, // Left Face
		1, 2, 5, 6 // Right Face

	};

        /// <summary>
        /// Lookup table for the UVs of a cube
        /// </summary>
        public static readonly Vector2[] VoxelUvs = new Vector2[4] {

        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 1.0f)

    };
    }
}
