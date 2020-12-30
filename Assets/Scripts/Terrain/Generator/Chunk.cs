using System;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEditor;
using System.Threading;

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
        /// 
        /// Note that this isn't WORLD space, rather a local chunk system, where the first chunk is (0, 0) and the next is (0, 1)
        /// 
        /// </summary>
        public int x;

        /// <summary>
        /// The Z value of the coord
        /// 
        /// Note that this isn't WORLD space, rather a local chunk system, where the first chunk is (0, 0) and the next is (0, 1)
        /// 
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

        public static bool operator ==(ChunkCoord first, ChunkCoord second)
        {
            return first.x == second.x && first.z == second.z;
        }

        public static bool operator !=(ChunkCoord first, ChunkCoord second)
        {
            return first.x != second.x || first.z != second.z;
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
        private byte[] voxelData;

        /// <summary>
        /// The coordinate of the chunk
        /// </summary>
        public ChunkCoord chunkCoord;

        /// <summary>
        /// The world space position of the chunk as an int3
        /// 
        /// This is only used for the burst C# jobs as int3s are more optimized when it comes to calculations versus Vector3s
        /// </summary>
        private int3 JWorldPos => new int3(Mathf.FloorToInt(chunkCoord.x * chunkSizeX), 0, Mathf.FloorToInt(chunkCoord.z * chunkSizeZ));

        /// <summary>
        /// The world space position of the chunk as a Vector3
        /// </summary>
        private Vector3Int WorldPos => Vector3Int.FloorToInt(new Vector3(chunkCoord.x * chunkSizeX, 0, chunkCoord.z * chunkSizeZ));

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
        public bool generated = false;

        private bool threadFinished = false;

        /// <summary>
        /// A reference to the <see cref="TerrainGenerator"/> <see cref="GameObject"/>, attached when created
        /// </summary>
        public TerrainGenerator generator;

        private bool dirty = false;

        public void Start()
        {
            // Register chunk in TerrainGenerator
            generator.chunks[chunkCoord.x, chunkCoord.z] = this;

            // Set chunkSizes, split into components to maximize speed
            chunkSizeX = generator.chunkXSize;
            chunkSizeY = generator.chunkYSize;
            chunkSizeZ = generator.chunkZSize;

            // Begin initial regeneration
            Regenerate();
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
                ConstructMesh();
                generated = true;


            }
        }

        public void Regenerate()
        {


            // Start chunk generation
            generated = false;
            threadFinished = false;

            voxelData = new byte[generator.chunkXSize * generator.chunkYSize * generator.chunkZSize];

            //PrepareMesh(null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(PrepareMesh));

        }

        private void PrepareMesh(object callback)
        {

            GetBlockData();
            CreateChunkMeshData(null);
        }

        private void OnDestroy()
        {
            // Ensures the chunk has been removed when destroyed
            if (generator != null && generator.chunks != null)
                generator.chunks[chunkCoord.x, chunkCoord.z] = null;
        }


        /// <summary>
        /// Fill chunk with voxel type data
        /// </summary>
        private void GetBlockData()
        {


            for (int x = 0; x < chunkSizeX; x++)
            {
                for (int y = 0; y < chunkSizeY; y++)
                {
                    for (int z = 0; z < chunkSizeZ; z++)
                    {
                        // Calculate 3D index from 1D index
                        int3 pos = new int3(x, y, z);

                        // Set voxel data from TerrainGenerator generation
                        voxelData[GetVoxelDataIndex(x, y, z)] = TerrainGenerator.GenerateVoxelType(pos + JWorldPos);
                    }
                }
            }






            //j_voxels = new NativeArray<byte>(voxelData, Allocator.TempJob);

            //j_chunkJob = new ChunkNoiseJob
            //{
            //    voxels = j_voxels,
            //    worldPos = JWorldPos,
            //    chunkSizeX = chunkSizeX,
            //    chunkSizeY = chunkSizeY,
            //    chunkSizeZ = chunkSizeZ
            //};

            //// Schedule job with a batchsize of 32
            //jHandle = j_chunkJob.Schedule(voxelData.Length, 32);

            //// Complete the job
            //jHandle.Complete();
        }

        /// <summary>
        /// Checks whether a block is solid or not
        /// 
        /// Checks adjacent chunks if the block isn't inside this one
        /// </summary>
        /// <param name="pos">The position of the cube</param>
        /// <returns>Whether the block needs to be rendered or not</returns>
        private bool CheckBlock(Vector3Int pos)
        {

            if (!VoxelInsideChunk(pos))
            {
                return generator.blockTypes[generator.GetVoxel(pos + WorldPos)].isSolid;
            }


            return generator.blockTypes[GetVoxelData(pos.x, pos.y, pos.z)].isSolid;
        }

        /// <summary>
        /// Adds all block data to a chunk
        /// </summary>
        private void CreateChunkMeshData(object callback)
        {


            for (int x = 0; x < chunkSizeX; x++)
            {
                //TerrainGenerator.threadCount++;


                for (int y = 0; y < chunkSizeY; y++)
                {
                    for (int z = 0; z < chunkSizeZ; z++)
                    {
                        //new Vector3Int(x, y, z);
                        //TerrainGenerator.threadCount++;
                        AddBlockData(new Vector3Int(x, y, z));
                    }
                }
            }


            //TerrainGenerator.threadCount++;
            // Thread is now finished, signal main thread
            threadFinished = true;
        }

        /// <summary>
        /// Adds all vertex, triangle, and UV data to a cube position based on certain conditions
        /// </summary>
        /// <param name="cubePos"></param>
        private void AddBlockData(Vector3Int cubePos)
        {

            // If the block isn't solid don't try rendering it
            if (!generator.blockTypes[GetVoxelData(cubePos.x, cubePos.y, cubePos.z)].isSolid) return;
            //if (!CheckBlock(cubePos)) return;

            // Loop through every side of the voxel
            for (int p = 0; p < 6; p++)
            {
                // Checks each side whether it contains a block, used for omitting sides that don't need to be rendered
                if (!CheckBlock(cubePos + VoxelTables.faces[p]))
                {

                    // 0, 0, 0 ; face 0 
                    // vert 1 - (0, 0, 0)
                    // vert 2 - (0, 1, 0)
                    // vert 3 - (1, 0, 0)
                    // vert 4 - (1, 1, 0)


                    // Add 4 vertices of cube side
                    vertices.Add(cubePos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p, 0]]);
                    vertices.Add(cubePos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p, 1]]);
                    vertices.Add(cubePos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p, 2]]);
                    vertices.Add(cubePos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p, 3]]);

                    // Adds designated texture side based on ID (UVs)
                    AddTexture(generator.blockTypes[GetVoxelData(cubePos.x, cubePos.y, cubePos.z)].GetTextureSide(p));

                    // Find x and y in relation to the texture 
                    float y = cubePos.z / (float)chunkSizeZ; //because uvs start top left, y needs to be inverted
                    float x = cubePos.x / (float)chunkSizeX;

                    float yAmount = 1f / chunkSizeZ;
                    float xAmount = 1f / chunkSizeX;

                    uvs.Add(new Vector2(x, y)); // 0, 0 - bottom left
                    uvs.Add(new Vector2(x, y + yAmount)); //0, 1 - top left
                    uvs.Add(new Vector2(x + xAmount, y)); // 1, 0 - bottom right
                    uvs.Add(new Vector2(x + xAmount, y + yAmount)); // 1, 1 - top right

                    // Add triangles
                    triangles.Add(vIndex);
                    triangles.Add(vIndex + 1);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 2);
                    triangles.Add(vIndex + 1);
                    triangles.Add(vIndex + 3);


                    // Increment index for next faces
                    vIndex += 4;
                }
            }


        }

        /// <summary>
        /// Constructs mesh from voxel data
        /// </summary>
        private void ConstructMesh()
        {
            // Add Mesh properties
            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };

            // Recalculate normals of the mesh
            mesh.RecalculateNormals();

            MeshFilter filter = GetComponent<MeshFilter>();
            MeshRenderer renderer = GetComponent<MeshRenderer>();

            // Set mesh to the GO and add the spritemap material from TerrainGenerator
            filter.mesh = mesh;
            renderer.material = TerrainGenerator.material;


        }

        /// <summary>
        /// Adds the designated UVs to a face of a voxel
        /// </summary>
        /// <param name="id">The texture ID, designated by the tile sprite image (first texture is id 0, 1, 2..)</param>
        private void AddTexture(int id)
        {

        }

        /// <summary>
        /// Return whether a given voxel position is within the chunk
        /// </summary>
        /// <param name="pos">The position of the chunk</param>
        /// <returns></returns>
        public bool VoxelInsideChunk(Vector3 pos)
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
        /// use <see cref="TerrainGenerator.GetVoxel(Vector3Int)"/>.
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