//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using TerrainTypes;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace TerrainGeneration
{
    /// <summary>
    /// Represents a chunk's X and Z in chunk coordinates rather than world space
    /// </summary>
    [Serializable]
    public struct ChunkCoord
    {
        /// <summary>
        /// The X value of the coord
        /// Note that this isn't WORLD space, rather a local chunk system, where the first chunk is (0, 0) and the next is (0, 1)
        /// </summary>
        public int x;

        /// <summary>
        /// The Z value of the coord
        /// Note that this isn't WORLD space, rather a local chunk system, where the first chunk is (0, 0) and the next is (0, 1)
        public int z;

        public ChunkCoord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoord coord &&
                   x == coord.x &&
                   z == coord.z;
        }

        /// <summary>
        /// Provide a way of comparing ChunkCoord objects by only checking their x and z variables.
        /// </summary>
        /// <returns>The hash code of the object</returns>
        public override int GetHashCode()
        {
            int hashCode = 1553271884;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Finds the distance between a chunk and location and checks if it's below threshold distance/>
        /// </summary>
        /// <param name="other">The <see cref="ChunkCoord"/> to be compared</param>
        /// <param name="distance">The distance the position should be less than</param>
        /// <returns>Whether the condition is fulfilled or not</returns>
        public bool IsDistanceFrom(ChunkCoord other, double distance)
        {
            //All in one line to avoid creating any variables, just modified distance squared formula
            return Math.Abs(x - other.x) < distance && Math.Abs(z - other.z) < distance;
            //return ((x - other.x) * (x - other.x)) < distance * distance && ((z - other.z) * (z - other.z)) < distance * distance;
        }

        /// <summary>
        /// Finds the approximate distance between two chunks. This is used to calculate the priority a chunk should have for being loaded.
        /// A chunk further away will have a lower priority, and a chunk closer will have a higher priority.
        /// </summary>
        /// <param name="other">The other <see cref="ChunkCoord"/> to compare</param>
        /// <returns>The distance between the two chunks squared</returns>
        public int Distance(ChunkCoord other)
        {
            return Math.Abs(x - other.x) + Math.Abs(z - other.z);
        }

        public static bool operator ==(ChunkCoord first, ChunkCoord second)
        {
            return first.x == second.x && first.z == second.z;
        }

        public static bool operator !=(ChunkCoord first, ChunkCoord second)
        {
            return first.x != second.x || first.z != second.z;
        }

        /// <summary>
        /// Offsets a <see cref="ChunkCoord"/> from an existing position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns>A new <see cref="ChunkCoord"/> representing the transform</returns>
        public ChunkCoord add(int x, int z)
        {
            return new ChunkCoord(this.x + x, this.z + z);
        }

        /// <summary>
        /// Converts from the chunk coordinate system to world space using the terrain chunk size.
        /// </summary>
        /// <returns>The world space location in the form of a <see cref="Vector3"/></returns>
        public Vector3 ToWorldSpace()
        {
            return new Vector3(x * TerrainGenerator.Instance.chunkXSize, 0, z * TerrainGenerator.Instance.chunkZSize);
        }

        public override string ToString()
        {
            return $"({x}, {z})";
        }
    }

    /// <summary>
    /// Represents a section of terrain containing many <see cref="BlockType"/>s
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        /// <summary>
        /// Stores all of the voxel block data in the chunk
        /// </summary>
        public Voxel[] voxelData;

        /// <summary>
        /// The coordinate of the chunk
        /// </summary>
        public ChunkCoord chunkCoord;

        /// <summary>
        /// The world space position of the chunk as a Vector3
        /// </summary>
        private int3 WorldPos => new int3(chunkCoord.x * chunkSizeX, 0, chunkCoord.z * chunkSizeZ);

        /// <summary>
        /// A list of the vertices in the mesh
        /// </summary>
        private readonly List<Vector3> vertices = new List<Vector3>();

        /// <summary>
        /// A list of the triangles in the mesh
        /// </summary>
        private readonly List<int> triangles = new List<int>();

        /// <summary>
        /// A list of the UV Coordinates in the mesh
        /// </summary>
        private readonly List<Vector2> uvs = new List<Vector2>();

        private ChunkManager chunkManager = new ChunkManager();
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        // Chunk size reference is broken into its components to minimize field references (Vector3.x, Vector3.y, etc)

        /// <summary>
        /// The length of a chunk in voxels, defined by the user in <see cref="TerrainGenerator"/>
        /// </summary>
        private int chunkSizeX;

        /// <summary>
        /// The maximum height of a chunk in voxels, defined by the user in <see cref="TerrainGenerator"/>
        /// </summary>
        private int chunkSizeY;

        /// <summary>
        /// The width of a chunk in voxels, defined by the user in <see cref="TerrainGenerator"/>
        /// </summary>
        private int chunkSizeZ;

        /// <summary>
        /// Used for determining whether a chunk has finished its generation process. Used to avoid memory leaks and 
        /// several other bugs
        /// </summary>
        public bool generated;

        // Job Handlers
        ChunkBuilder.ChunkNoiseHandler noiseHandler;
        ChunkBuilder.ChunkMeshHandler meshHandler;

        /// <summary>
        /// A reference to the <see cref="TerrainGenerator"/> <see cref="GameObject"/>, attached when created
        /// </summary>
        public TerrainGenerator generator = TerrainGenerator.Instance;

        // Used for determining whether a chunk needs to be regenerated or not. If the chunk is marked as dirty, the method GenerateChunk will be called the next frame.
        public bool dirty = false;

        public void OnEnable()
        {
            // Call GetComponent for renderer and filter if not loaded yet
            if (meshRenderer == null)
            {
                meshCollider = GetComponent<MeshCollider>();
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();
            }



            // Set chunkSizes, split into components to maximize speed
            chunkSizeX = generator.chunkXSize;
            chunkSizeY = generator.chunkYSize;
            chunkSizeZ = generator.chunkZSize;

            // Begin initial regeneration
            Regenerate();


        }

        /// <summary>
        /// Equivalent to the normal <see cref="OnDestroy"/>, but disable is used because of chunks being managed by <see cref="ObjectPoolManager"/>
        /// </summary>
        public void OnDisable()
        {
            noiseHandler?.jobHandle.Complete();
            noiseHandler?.DisposeNatives();
            noiseHandler = null;
            meshHandler?.jobHandle.Complete();
            meshHandler?.DisposeNatives();
            meshHandler = null;

            ClearChunk(false);
        }

        /// <summary>
        /// Clears all existing values in the chunk for regeneration.
        /// </summary>
        public void ClearChunk(bool chunkRegenerate)
        {
            dirty = false;
            generated = false;
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            chunkManager.OnChunkUnloaded(chunkCoord, chunkRegenerate);
        }

        private void Update()
        {
            // Chunk is marked for regeneration, start the process
            if (dirty)
            {
                Debug.Log("Regenerating...");
                Regenerate();
                dirty = false;
            }

            if (generated)
            {
                if (chunkCoord.IsDistanceFrom(TerrainGenerator.Instance.lastChunkPos, 3))
                {

                    if (meshCollider.sharedMesh == null)
                    {
                        //Debug.Log("Creating shared mesh");
                        // If the mesh is inside of the distance and needs to be loaded, set the physics mesh
                        meshCollider.sharedMesh = meshFilter.mesh;
                    }
                }
                else
                {
                    if (meshCollider.sharedMesh != null)
                    {
                        // If the mesh is outside of the distance, no need to load its physics
                        meshCollider.sharedMesh = null;
                    }
                }
            }
        }

        /// <summary>
        /// Regenerates the mesh, first clearing the values and then starting the <see cref="Coroutine"/> <see cref="GenerateChunk"/>.
        /// </summary>
        public void Regenerate()
        {
            ClearChunk(true);

            // Start terrain generation 
            StartCoroutine(GenerateChunk());
            //new Task(() => PrepareMesh()).Start();
        }

        /// <summary>
        /// Attempts to generate the mesh of the chunk.
        /// 
        /// If the noise data has been cached in the <see cref="TerrainGenerator.chunks"/> array, the <see cref="ChunkBuilder.ChunkNoiseJob"/> will be skipped.
        /// </summary>
        private IEnumerator GenerateChunk()
        {
            generated = false;

            noiseHandler = new ChunkBuilder.ChunkNoiseHandler(chunkSizeX, chunkSizeY, chunkSizeZ);

            byte[] byteData = new byte[generator.chunkXSize * generator.chunkYSize * generator.chunkZSize];

            Chunk chunk = generator.chunks[chunkCoord.x, chunkCoord.z];

            if (chunk == null || chunk.voxelData == null || byteData == null || chunk.voxelData[0] == null)
            {
                // No voxel data has been generated, start the noise job
                voxelData = new Voxel[generator.chunkXSize * generator.chunkYSize * generator.chunkZSize];

                yield return noiseHandler.StartNoiseJob(byteData);

                for (int i = 0; i < byteData.Length; i++)
                {
                    voxelData[i] = new Voxel(byteData[i], generator.voxelTypes[byteData[i]]);
                }

            }
            else
            {
                Voxel[] newVoxelData = chunk.voxelData;

                //Debug.Log("Already found data!");
                // Already has chunk data, just set the data instead
                // TODO: Possibly find a better way of structuring the data?
                for (int i = 0; i < newVoxelData.Length; i++)
                {
                    if (voxelData[i] is MachineSlaveVoxel)
                    {
                        byteData[i] = 0;
                    }
                    else
                    {
                        byteData[i] = newVoxelData[i].value;
                    }
                }
            }

            meshHandler = new ChunkBuilder.ChunkMeshHandler(chunkSizeX, chunkSizeY, chunkSizeZ);
            yield return meshHandler.StartMeshJob(byteData);



            Mesh mesh = meshHandler.GetMeshData();

            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            mesh.RecalculateBounds();
            // Recalculate normals of the mesh
            mesh.RecalculateNormals();

            // Set mesh to the GO and add the spritemap material from TerrainGenerator
            meshFilter.mesh = mesh;
            meshRenderer.material = TerrainGenerator.Material;

            generated = true;

            // Register chunk in TerrainGenerator
            generator.chunks[chunkCoord.x, chunkCoord.z] = this;

            chunkManager.OnChunkLoaded(this);
        }

        /// <summary>
        /// Checks whether a block is solid or not.
        /// Checks adjacent chunks if the block isn't inside this one.
        /// </summary>
        /// <param name="pos">The position of the cube</param>
        /// <returns>Whether the block needs to be rendered or not</returns>
        bool CheckBlock(int3 pos)
        {
            if (!VoxelInsideChunk(pos))
            {
                return generator.voxelTypes[generator.GetVoxelValue(pos + WorldPos)].isSolid;


                //return generator.voxelTypes[generator.GetVoxelValue(pos + WorldPos)].isSolid;
            }

            return generator.voxelTypes[GetVoxelData(pos.x, pos.y, pos.z)].isSolid;
        }

        /// <summary>
        /// Return whether a given voxel position is within the chunk
        /// </summary>
        /// <param name="pos">The position of the chunk</param>
        /// <returns></returns>
        public bool VoxelInsideChunk(int3 pos)
        {
            return VoxelInsideChunk(pos.x, pos.y, pos.z);
        }

        /// <summary>
        /// Return whether a given voxel position is within the chunk
        /// </summary>
        /// <param name="pos">The position of the chunk</param>
        /// <returns></returns>
        public bool VoxelInsideChunk(int x, int y, int z)
        {
            return !(x < 0 || x > chunkSizeX - 1 || y < 0 || y > chunkSizeY - 1 || z < 0 || z > chunkSizeZ - 1);
        }

        /// <summary>
        /// Converts a 3D index into a 1D, used for indexing compressed voxel array
        /// </summary>
        /// <param name="x">The local x coordinate of the voxel</param>
        /// <param name="y">The local y coordinate of the voxel</param>
        /// <param name="z">The local z coordinate of the voxel</param>
        /// <returns>A 1D index to be used for <see cref="voxelData"/> primarily</returns>
        private int GetVoxelDataIndex(int x, int y, int z)
        {
            return (z * chunkSizeX * chunkSizeY) + (y * chunkSizeX) + x;
        }

        /// <summary>
        /// Retrieves the <see cref="Block"/> INSIDE the chunk. For retrieval of voxels outside 
        /// use <see cref="TerrainGenerator.GetVoxelValue(int3)"/>.
        /// </summary>
        /// <param name="x">The local x coordinate of the voxel</param>
        /// <param name="y">The local y coordinate of the voxel</param>
        /// <param name="z">The local z coordinate of the voxel</param>
        /// <returns>The corresponding material type for the voxel</returns>
        public Voxel GetVoxel(int x, int y, int z)
        {
            return voxelData[GetVoxelDataIndex(x, y, z)];
        }

        /// <summary>
        /// Retrieves the int voxel value of a block INSIDE the chunk. For retrieval of voxels outside 
        /// use <see cref="TerrainGenerator.GetVoxelValue(int3)"/>.
        /// </summary>
        /// <param name="x">The local x coordinate of the voxel</param>
        /// <param name="y">The local y coordinate of the voxel</param>
        /// <param name="z">The local z coordinate of the voxel</param>
        /// <returns>The corresponding material type for the voxel</returns>
        public byte GetVoxelData(int x, int y, int z)
        {
            return voxelData[GetVoxelDataIndex(x, y, z)].value;
        }

        /// <summary>
        /// Sets the <see cref="Voxel"/> at a given local space coordinate.
        /// </summary>
        /// <param name="x">The local x coordinate of the voxel</param>
        /// <param name="y">The local y coordinate of the voxel</param>
        /// <param name="z">The local z coordinate of the voxel</param>
        /// <param name="newVoxel"></param>
        public void SetVoxelData(int x, int y, int z, Voxel newVoxel)
        {
            voxelData[GetVoxelDataIndex(x, y, z)] = newVoxel;
        }

        /// <summary>
        /// Sets a rectangular region to a given <see cref="Voxel"/>.
        /// </summary>
        /// <param name="lowerBoundX"></param>
        /// <param name="lowerBoundY"></param>
        /// <param name="lowerBoundZ"></param>
        /// <param name="upperBoundX"></param>
        /// <param name="upperBoundY"></param>
        /// <param name="upperBoundZ"></param>
        /// <param name="newVoxel">The new voxel to fill the region</param>
        public void SetVoxelRegion(int lowerBoundX, int lowerBoundY, int lowerBoundZ, int upperBoundX, int upperBoundY,
            int upperBoundZ, Voxel newVoxel)
        {
            SetVoxelRegion(new int3(lowerBoundX, lowerBoundY, lowerBoundZ), new int3(upperBoundX, upperBoundY, upperBoundZ), newVoxel);
        }

        /// <summary>
        /// Sets a rectangular region to a given <see cref="Voxel"/>.
        /// </summary>
        /// <param name="lowerBound">One corner of the region</param>
        /// <param name="upperBound">The other corner of the region</param>
        /// <param name="newVoxel">The new voxel to fill the region</param>
        public void SetVoxelRegion(int3 lowerBound, int3 upperBound, Voxel newVoxel)
        {
            for (int x = lowerBound.x; x <= upperBound.x; x++)
            {
                for (int y = lowerBound.y; y <= upperBound.y; y++)
                {
                    for (int z = lowerBound.z; z <= upperBound.z; z++)
                    {
                        if (!VoxelInsideChunk(x, y, z))
                        {
                            int3 localPos = generator.GetRelativeChunkPosition(x, y, z);
                            generator.currentChunks[generator.GetChunkCoord(x, y, z)].SetVoxelData(localPos.x, localPos.y, localPos.z, newVoxel);
                        }
                        else
                        {
                            SetVoxelData(x, y, z, newVoxel);
                        }
                    }
                }
            }
        }

        //public void OnDrawGizmos()
        //{
        //    //TODO: Move to a debug button?
        //    //Drawing voxels
        //    Gizmos.color = Color.blue;

        //    if (generated)
        //    {
        //        for (int x = 0; x < chunkSizeX; x++)
        //        {
        //            for (int y = 0; y < chunkSizeY; y++)
        //            {
        //                for (int z = 0; z < chunkSizeZ; z++)
        //                {

        //                    Gizmos.color = Color.green;

        //                    Voxel b = voxelData[GetVoxelDataIndex(x, y, z)];

        //                    if (b is Voxel) Gizmos.color = Color.red;

        //                    if (b is MachineSlaveVoxel)
        //                    {
        //                        Gizmos.DrawWireCube(new Vector3(x + 0.5f + chunkCoord.x * chunkSizeX, y + 0.5f, chunkCoord.z * chunkSizeZ + z + 0.5f), Vector3.one);
        //                    }


        //                }
        //            }
        //        }
        //    }


        //}
    }
}
