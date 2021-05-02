//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Priority_Queue;
using TerrainGeneration;
using TerrainTypes;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace TerrainGeneration
{
    /// <summary>
    /// Generates procedural voxel terrain based on values provided
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance accessor for <see cref="TerrainGenerator"/>
        /// </summary>
        public static TerrainGenerator Instance;

        [Tooltip("A prefab for caching chunks")]
        public GameObject emptyChunk;

        [Header("Terrain Properties")]
        [Tooltip("The amount of chunks (l x w) in the world")]
        public Vector2Int worldSize;

        [Header("Chunk Properties")]
        [Tooltip("How long the chunks should be in width")]
        public int chunkXSize = 64;
        [Tooltip("How long the chunks should be in height")]
        public int chunkYSize = 64;
        [Tooltip("How long the chunks should be in length")]
        public int chunkZSize = 64;

        [Header("Determines how many chunks around the player should be loaded at any given time")]
        public int chunkRange = 3;
        [HideInInspector] public bool forceChunkCheck;

        [Tooltip("The tileset to be used for the terrain")]
        public Material cubeMaterial;

        [Tooltip("The player's GameObject")]
        public Transform player;

        /// <summary>
        /// The current list of chunks in the world, used for indexing later on
        /// </summary>
        public Chunk[,] chunks;

        /// <summary>
        /// A list of chunks currently in the world
        /// </summary>
        public Dictionary<ChunkCoord, Chunk> currentChunks = new Dictionary<ChunkCoord, Chunk>();

        /// <summary>
        /// The tilemap of the terrain
        /// </summary>
        public static Material material;

        /// <summary>
        /// Contains a list of <see cref="ChunkCoord"/>s to be loaded
        /// </summary>
        private FastPriorityQueue<ChunkCoordNode> chunkQueue = new FastPriorityQueue<ChunkCoordNode>(100000);

        /// <summary>
        /// Represents the last chunk position the player was at
        /// 
        /// Used for avoiding unnecessary chunk checks for unloading
        /// </summary>
        public ChunkCoord lastChunkPos;

        /// <summary>
        /// List of all the voxel types in the game
        /// </summary>
        public VoxelType[] voxelTypes;

        /// <summary>
        /// Caches the last player pos to avoid grabbing its component
        /// </summary>
        private Vector3 lastPlayerPos;

        /// <summary>
        /// Noise generation class
        /// See https://github.com/Auburn/FastNoise/blob/master/CSharp/README.md for more information
        /// </summary>
        private static FastNoiseLite noise;

        private void Awake()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
            Instance = this;
        }

        private class ChunkCoordNode : FastPriorityQueueNode
        {
            public ChunkCoord coord;
        }


        /// <summary>
        /// Starts the chunk updating process
        /// </summary>
        private void Start()
        {

            ObjectPoolManager.Instance.CreatePool(emptyChunk, 200);

            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            chunks = new Chunk[worldSize.x, worldSize.y];

            //ObjectPoolManager.instance.CreatePool(emptyChunk, 100);

            material = cubeMaterial;

            //for (int x = 0; x < 100; x++)
            //{
            //    for (int z = 0; z < 100; z++)
            //    {
            //        ChunkCoord next = new ChunkCoord(x, z);

            //        //GameObject go = Instantiate(emptyChunk, new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize), Quaternion.identity);
            //        GameObject go = ObjectPoolManager.Instance.ReuseObject(emptyChunk, new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize), Quaternion.identity, false);

            //        Chunk chunk = go.GetComponent<Chunk>();

            //        if (chunk == null)
            //        {
            //            // Pooled object doesn't have a chunk component yet, add it
            //            chunk = go.AddComponent<Chunk>();
            //        }

            //        chunk.chunkCoord = next;

            //        chunks[next.x, next.z] = chunk;
            //        currentChunks[next] = chunk;

            //        chunk.generator = this;

            //        // All data has been updated, now it can be reactivated
            //        go.SetActive(true);
            //    }
            //}




            StartCoroutine(UpdateChunks());
        }

        /// <summary>
        /// Loads and unloads chunks based on the player's position
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateChunks()
        {
            while (true)
            {

                for (int i = 0; i < 20; i++) //loads i chunks every frame
                {
                    if (chunkQueue.Count != 0)
                    {
                        ChunkCoordNode nextNode = chunkQueue.Dequeue();
                        ChunkCoord next = nextNode.coord;


                        if (!next.IsDistanceFrom(GetChunkCoord(lastPlayerPos), chunkRange)) continue;

                        if (!currentChunks.ContainsKey(next)) // Prevents chunks from being generated so fast they duplicate
                        {
                            //GameObject go = Instantiate(emptyChunk, new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize), Quaternion.identity);

                            GameObject go;
                            if (chunks[next.x, next.z] != null)
                            {
                                go = chunks[next.x, next.z].gameObject;
                                go.SetActive(true);
                            }
                            else
                            {
                                go = ObjectPoolManager.Instance.ReuseObject(emptyChunk, new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize), Quaternion.identity, false);
                            }


                            Chunk chunk = go.GetComponent<Chunk>();

                            if (chunk == null)
                            {
                                // Pooled object doesn't have a chunk component yet, add it
                                chunk = go.AddComponent<Chunk>();
                            }

                            chunk.chunkCoord = next;

                            chunks[next.x, next.z] = chunk;
                            currentChunks[next] = chunk;

                            chunk.generator = this;

                            // All data has been updated, now it can be reactivated
                            go.SetActive(true);
                        }
                    }

                }


                yield return null;
            }
        }

        /// <summary>
        /// Loads/Unloads chunks around the player
        /// </summary>
        private void Update()
        {
            lastPlayerPos = player.position;
            // Retrieve the coord for the player
            //ChunkCoord playerPos = new ChunkCoord { x = 7, z = 4 };
            ChunkCoord playerPos = GetChunkCoord(lastPlayerPos);

            foreach (ChunkCoordNode queuedChunk in chunkQueue)
            {
                if (!queuedChunk.coord.IsDistanceFrom(lastChunkPos, chunkRange))
                {
                    chunkQueue.Remove(queuedChunk);
                }
                else
                {
                    chunkQueue.UpdatePriority(queuedChunk, queuedChunk.coord.Distance(lastChunkPos));
                }
            }

            // Avoids unnecessary checks by only updating once the player has reached a new chunk
            if (playerPos != lastChunkPos || forceChunkCheck)
            {
                forceChunkCheck = false;

                // Loop through every chunk around player
                for (int x = playerPos.x - chunkRange; x < playerPos.x + chunkRange; x++)
                {
                    for (int z = playerPos.z - chunkRange; z < playerPos.z + chunkRange; z++)
                    {
                        ChunkCoord coord = new ChunkCoord { x = x, z = z };

                        if (currentChunks.TryGetValue(coord, out Chunk c)) continue; // Chunk has already been loaded

                        ChunkCoordNode node = new ChunkCoordNode() { coord = coord };

                        if (chunkQueue.Contains(node)) continue;

                        if (coord.x < 0 || coord.x > worldSize.x - 1 || coord.z < 0 || coord.z > worldSize.y - 1) continue; // Chunk doesn't exist

                        chunkQueue.Enqueue(node, coord.Distance(lastChunkPos)); // Queue chunk for loading


                    }
                }

                foreach (var pair in currentChunks.Where(p =>
                {
                    ChunkCoord coord = p.Key;

                    if (!coord.IsDistanceFrom(playerPos, chunkRange))
                    {

                        if (chunks[coord.x, coord.z] == null) return true;
                        //chunks[coord.x, coord.z].gameObject.component
                        //Destroy(p.Value.gameObject);
                        //ObjectPoolManager.Instance.DestroyObject(currentChunks[coord].gameObject);
                        currentChunks[coord].gameObject.SetActive(false);
                        //chunks[coord.x, coord.z] = null;

                        //chunks[coord.x, coord.z].chunkGO.SetActive(false)
                        return true;
                    }
                    return false;
                }).ToList())
                {
                    currentChunks.Remove(pair.Key);
                }


                // Unload all chunks that are a certain distance away from the player

            }

            lastChunkPos = playerPos;
        }

        /// <summary>
        /// Retrieves a chunk coord based off of a position
        /// </summary>
        /// <param name="pos">The location to be converted</param>
        /// <returns>A chunk coord based on the <see cref="chunkXSize"/> and <see cref="chunkZSize"/></returns>
        public ChunkCoord GetChunkCoord(float x, float y, float z)
        {
            ChunkCoord coord = new ChunkCoord { x = Mathf.FloorToInt(x / chunkXSize), z = Mathf.FloorToInt(z / chunkZSize) };
            return coord;
        }

        /// <summary>
        /// Retrieves a chunk coord based off of a <see cref="float3"/> position
        /// </summary>
        /// <param name="pos">The location to be converted</param>
        /// <returns>A chunk coord based on the <see cref="chunkXSize"/> and <see cref="chunkZSize"/></returns>
        public ChunkCoord GetChunkCoord(float3 pos)
        {
            return GetChunkCoord(pos.x, pos.y, pos.z);
        }

        /// <summary>
        /// Uses noise functions to generate the Voxel type for a voxel
        /// 
        /// This is used primarily for the <see cref="Chunk"/> generation C# jobs.
        /// </summary>
        /// <param name="pos">The position of the voxel in world space</param>
        /// <returns></returns>
        public static byte GenerateVoxelType(int posX, int posY, int posZ)
        {

            // TERRAIN GENERATION CODE

            int height = (int)(noise.GetNoise(posX * .1f, posZ * .1f) * 3 + 13); // Noise method

            byte val = 2; // By default type 2

            if (posY > height) val = 0; // Greater than height, should be air
            if (posY == height) val = 1; // Equal to the height, make it grass

            return val;
        }

        /// <summary>
        /// Retrieves the <see cref="Voxel"/> at a given world space location.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>A <see cref="Voxel"/> at a given point. If the voxel is out of the world bounds, the method will return null.</returns>
        public Voxel GetVoxel(int3 pos)
        {
            int posX = pos.x;
            int posY = pos.y;
            int posZ = pos.z;

            if (posX < 0 || posX > worldSize.x * chunkXSize - 1 || posY < 0 || posY > chunkYSize - 1 || posZ < 0 || posZ > worldSize.y * chunkZSize - 1)
                return null;

            //return GenerateVoxelType(new int3(posX, posY, posZ));

            ChunkCoord coord = GetChunkCoord(pos);


            Chunk foundChunk = chunks[coord.x, coord.z];

            // Chunk is already loaded
            //if (foundChunk != null && foundChunk.generated) // original version
            if (foundChunk != null) // changed to this otherwise IOs wouldn't connect
            {
                // TODO: Convert relative chunk lookup to new method made GetRelativeChunkPosition(int, int, int)
                return foundChunk.GetVoxel(posX - (coord.x * chunkXSize), posY, posZ - (coord.z * chunkZSize)); // Return the byte value in the chunk to avoid extra noise call
            }

            // Chunk hasn't been generated yet, return null
            return null;
        }


        /// <summary>
        /// Retrieves the current voxel type byte value of a position
        /// </summary>
        /// <param name="pos">The position of the voxel</param>
        /// <returns></returns>
        public byte GetVoxelValue(int3 pos)
        {
            Voxel voxel = GetVoxel(pos);

            // Check if no Voxel was found
            if (voxel == null)
            {
                // Chunk hasn't been generated yet, just generate the voxel
                // NOTE: If possible avoid having to pull Voxels from unloaded chunks
                return GenerateVoxelType(pos.x, pos.y, pos.z);
            }

            return voxel.value;
        }

        /// <summary>
        /// Retrieves the <see cref="VoxelType"/> of a position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public VoxelType GetVoxelType(int3 pos)
        {
            return voxelTypes[GetVoxelValue(pos)];
        }

        /// <summary>
        /// Converts a world space position into a local chunk position
        /// </summary>
        /// <param name="x">A world space x coordinate</param>
        /// <param name="y">A world space y coordinate</param>
        /// <param name="z">A world space z coordinate</param>
        /// <returns></returns>
        public int3 GetRelativeChunkPosition(int x, int y, int z)
        {
            return new int3(x % chunkXSize, y, z % chunkZSize);
        }
    }

    /// <summary>
    /// Represents a Voxel's ID and transparency, as well as texturing
    /// </summary>
    [System.Serializable]
    public struct VoxelType
    {
        //[Header("Voxel Properties")]
        //[Tooltip("The name of the Voxel")]
        //public string id;
        [Tooltip("Determines whether the Voxel is air or not")]
        public bool isSolid;
    }
}
