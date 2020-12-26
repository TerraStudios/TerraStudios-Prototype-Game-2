using UnityEngine;

namespace TerrainGeneration
{
	/// <summary>
	/// Stores constant data about each voxel (cube)
	/// </summary>
	static class VoxelTables
	{
		/// <summary>
		/// Size in pixels of a block in the sprite map
		/// </summary>
		public static readonly Vector2 BlockSize = new Vector2(128, 128);

		/// <summary>
		/// Amount of blocks in length of the sprite
		/// </summary>
		public static readonly int TextureAtlasScaleX = 1;//(int)(TerrainGenerator.material.mainTexture.width / BlockSize.x);

		/// <summary>
		/// Amount of blocks in width of the sprite
		/// </summary>
		public static readonly int TextureAtlasScaleY = 1;//(int)(TerrainGenerator.material.mainTexture.height / BlockSize.y);

		/// <summary>
		/// Normalizes the x texture size, used for calculating UVs
		/// </summary>
		public static float NormalizedTextureSizeX
		{
			get { return 1f / TextureAtlasScaleX; }
		}

		/// <summary>
		/// Normalizes the y texture size, used for calculating UVs
		/// </summary>
		public static float NormalizedTextureSizeY
		{
			get { return 1f / (float)TextureAtlasScaleY; }
		}

		/// <summary>
		/// Lookup table for the vertices in a cube
		/// </summary>
		public static readonly Vector3[] voxelVerts = new Vector3[8] {

		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f),

	};

		/// <summary>
		/// Lookup table for the faces of a cube
		/// </summary>
		public static readonly Vector3Int[] faces = new Vector3Int[6]
		{
		new Vector3Int(0, 0, -1),
		new Vector3Int(0, 0, 1),
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)
		};

		/// <summary>
		/// Lookup table for the triangles of a cube
		/// </summary>
		public static readonly int[,] voxelTris = new int[6, 4] {

		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

		/// <summary>
		/// Lookup table for the UVs of a cube
		/// </summary>
		public static readonly Vector2[] voxelUvs = new Vector2[4] {

		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 1.0f)

	};
	}
}