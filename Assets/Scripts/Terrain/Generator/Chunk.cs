//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using BuildingManagement;
using BuildingModules;
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
        /// I have no idea how this works but visual studio generated it for me
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
        /// Converts from the chunk coordinate system to world space using the terrain chunk size.
        /// </summary>
        /// <returns>The world space location in the form of a <see cref="Vector3"/></returns>
        public Vector3 ToWorldSpace()
        {
            return new Vector3(x * TerrainGenerator.instance.chunkXSize, 0, z * TerrainGenerator.instance.chunkZSize);
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
        public byte[] voxelData;

        /// <summary>
        /// The coordinate of the chunk
        /// </summary>
        public ChunkCoord chunkCoord;

        /// <summary>
        /// The world space position of the chunk as a Vector3
        /// </summary>
        private int3 WorldPos => new int3(chunkCoord.x * chunkSizeX, 0, chunkCoord.z * chunkSizeZ);

        /// <summary>
        /// The current vertex index, used for forming quads in mesh generation
        /// </summary>
        private int vIndex;

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

        private bool threadFinished;

        // Job Handlers
        ChunkBuilder.ChunkNoiseHandler noiseHandler;
        ChunkBuilder.ChunkMeshHandler meshHandler;

        /// <summary>
        /// A reference to the <see cref="TerrainGenerator"/> <see cref="GameObject"/>, attached when created
        /// </summary>
        public TerrainGenerator generator = TerrainGenerator.instance;

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

            // Register chunk in TerrainGenerator
            generator.chunks[chunkCoord.x, chunkCoord.z] = this;

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

            ClearChunk();
        }

        /// <summary>
        /// Clears all existing values in the chunk for regeneration.
        /// </summary>
        public void ClearChunk()
        {
            dirty = false;
            generated = false;
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            // Unload/disable all building mesh GameObjects in this chunk
            if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
            {
                Debug.Log("Destroying " + BuildingSystem.PlacedBuildings[chunkCoord].Count + " meshes!");

                for (int i = 0; i < BuildingSystem.PlacedBuildings[chunkCoord].Count; i++)
                {
                    KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunkCoord][i];

                    ObjectPoolManager.Instance.DestroyObject(kvp.Value);
                    // Make the corresponding mesh null so we don't somehow get an invalid one
                    BuildingSystem.PlacedBuildings[chunkCoord][i] = new KeyValuePair<Building, GameObject>(kvp.Key, null);
                }
            }

            vIndex = 0;
        }

        private void Update()
        {
            // Chunk is marked for regeneration, start the process
            if (dirty)
            {
                Regenerate();
                dirty = false;
            }

            if (!generated && threadFinished)
            {
                //ConstructMesh();
                generated = true;
            }

            if (generated)
            {
                if (chunkCoord.IsDistanceFrom(TerrainGenerator.instance.lastChunkPos, 3))
                {

                    if (meshCollider.sharedMesh == null)
                    {
                        Debug.Log("Creating shared mesh");
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

        public void Regenerate()
        {
            ClearChunk();

            voxelData = new byte[generator.chunkXSize * generator.chunkYSize * generator.chunkZSize];

            // Start terrain generation 


            StartCoroutine(GenerateChunk());

            // Load all building mesh GameObjects in this chunk
            if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
            {
                // Loop all placed buildings data in this chunk
                for (int i = 0; i < BuildingSystem.PlacedBuildings[chunkCoord].Count; i++)
                {
                    KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunkCoord][i];

                    Building building = kvp.Key;
                    //GameObject mesh = list[i].Value;
                    GameObject reused = ObjectPoolManager.Instance.ReuseObject(building.GetMeshObj(kvp.Key.scriptPrefabLocation).gameObject, kvp.Key.meshData.pos, kvp.Key.meshData.rot);
                    building.correspondingMesh = reused.transform;
                    // Overwriting the current KVP so we can Destroy it later with OPM
                    BuildingSystem.PlacedBuildings[chunkCoord][i] = new KeyValuePair<Building, GameObject>(building, reused);
                }

                Debug.Log("Enabled " + BuildingSystem.PlacedBuildings[chunkCoord].Count + " meshes!");
            }

            //new Task(() => PrepareMesh()).Start();
        }

        private IEnumerator GenerateChunk()
        {
            generated = false;

            noiseHandler = new ChunkBuilder.ChunkNoiseHandler(chunkSizeX, chunkSizeY, chunkSizeZ);
            yield return noiseHandler.StartNoiseJob(voxelData);

            meshHandler = new ChunkBuilder.ChunkMeshHandler(chunkSizeX, chunkSizeY, chunkSizeZ);
            yield return meshHandler.StartMeshJob(voxelData);

            Mesh mesh = meshHandler.GetMeshData();

            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            vIndex = 0;

            mesh.RecalculateBounds();
            // Recalculate normals of the mesh
            mesh.RecalculateNormals();

            // Set mesh to the GO and add the spritemap material from TerrainGenerator
            meshFilter.mesh = mesh;
            meshRenderer.material = TerrainGenerator.material;

            generated = true;
        }

        /// <summary>
        /// Checks whether a block is solid or not.
        /// Checks adjacent chunks if the block isn't inside this one.
        /// </summary>
        /// <param name="pos">The position of the cube</param>
        /// <returns>Whether the block needs to be rendered or not</returns>
        private bool CheckBlock(int3 pos)
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
            return !(pos.x < 0 || pos.x > chunkSizeX - 1 || pos.y < 0 || pos.y > chunkSizeY - 1 || pos.z < 0 || pos.z > chunkSizeZ - 1);
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
            return chunkSizeX * chunkSizeY * z + chunkSizeZ * y + x;
        }

        /// <summary>
        /// Retrieves the byte voxel value of a block INSIDE the chunk. For retrieval of voxels outside 
        /// use <see cref="TerrainGenerator.GetVoxelValue(int3)"/>.
        /// </summary>
        /// <param name="x">The local x coordinate of the voxel</param>
        /// <param name="y">The local y coordinate of the voxel</param>
        /// <param name="z">The local z coordinate of the voxel</param>
        /// <returns>The corresponding material type for the voxel</returns>
        public byte GetVoxelData(int x, int y, int z)
        {
            return voxelData[GetVoxelDataIndex(x, y, z)];
        }
    }
}
