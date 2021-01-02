using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TerrainGeneration;
using Unity.Mathematics;
using UnityEngine;

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
        public static TerrainGenerator instance;

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
        private Queue<ChunkCoord> chunkQueue = new Queue<ChunkCoord>();

        /// <summary>
        /// Represents the last chunk position the player was at
        /// 
        /// Used for avoiding unnecessary chunk checks for unloading
        /// </summary>
        private ChunkCoord lastChunkPos;

        /// <summary>
        /// List of all the voxel types in the game
        /// </summary>
        public VoxelType[] voxelTypes;

        /// <summary>
        /// Noise generation class
        /// See https://github.com/Auburn/FastNoise/blob/master/CSharp/README.md for more information
        /// </summary>
        private static FastNoiseLite noise;

        private void Awake()
        {
            instance = this;
        }


        /// <summary>
        /// Starts the chunk updating process
        /// </summary>
        private void Start()
        {
            noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            chunks = new Chunk[worldSize.x, worldSize.y];

            //ObjectPoolManager.instance.CreatePool(emptyChunk, 100);

            material = cubeMaterial;

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

                for (int i = 0; i < 4; i++) //loads i chunks every frame
                {
                    if (chunkQueue.Count != 0)
                    {
                        ChunkCoord next = chunkQueue.Dequeue();

                        if (!next.IsDistanceFrom(GetChunkCoord(player.position), chunkRange)) continue;

                        if (!currentChunks.ContainsKey(next)) // Prevents chunks from being generated so fast they duplicate
                        {
                            //Chunk chunk = ObjectPoolManager.instance.ReuseObject(emptyChunk, new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize), Quaternion.identity).GetComponent<Chunk>();
                            GameObject go = Instantiate(emptyChunk);

                            go.transform.parent = transform;

                            go.transform.position = new Vector3(next.x * chunkXSize, 0, next.z * chunkZSize);

                            Chunk chunk = go.GetComponent<Chunk>();

                            chunk.generator = this;
                            chunk.chunkCoord = next;

                            //new Chunk(this, next);

                            currentChunks[next] = chunk;
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
            Vector3 position = player.position;
            // Retrieve the coord for the player
            //ChunkCoord playerPos = new ChunkCoord { x = 7, z = 4 };
            ChunkCoord playerPos = GetChunkCoord(position);

            // Avoids unnecessary checks by only updating once the player has reached a new chunk
            if (playerPos != lastChunkPos)
            {

                // Loop through every chunk around player
                for (int x = playerPos.x - chunkRange; x < playerPos.x + chunkRange; x++)
                {
                    for (int z = playerPos.z - chunkRange; z < playerPos.z + chunkRange; z++)
                    {
                        ChunkCoord coord = new ChunkCoord { x = x, z = z };

                        if (currentChunks.ContainsKey(coord)) continue; // Chunk has already been loaded}


                        if (coord.x < 0 || coord.x > worldSize.x - 1 || coord.z < 0 || coord.z > worldSize.y - 1) continue; // Chunk doesn't exist


                        chunkQueue.Enqueue(coord); // Queue chunk for loading


                    }
                }

                foreach (var pair in currentChunks.Where(p =>
                {
                    ChunkCoord coord = p.Key;

                    if (!coord.IsDistanceFrom(playerPos, chunkRange))
                    {
                        Destroy(chunks[coord.x, coord.z].gameObject);
                        //ObjectPoolManager.instance.DestroyObject(chunks[coord.x, coord.z].gameObject);
                        chunks[coord.x, coord.z] = null;

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
        /// Retrieves a chunk coord based off of a <see cref="Vector3"/> position
        /// </summary>
        /// <param name="pos">The location to be converted</param>
        /// <returns>A chunk coord based on the <see cref="chunkXSize"/> and <see cref="chunkZSize"/></returns>
        public ChunkCoord GetChunkCoord(Vector3 pos)
        {
            return new ChunkCoord { x = Mathf.FloorToInt(pos.x / chunkXSize), z = Mathf.FloorToInt(pos.z / chunkZSize) };
        }

        /// <summary>
        /// Uses noise functions to generate the block type for a voxel
        /// 
        /// This is used primarily for the <see cref="Chunk"/> generation C# jobs.
        /// </summary>
        /// <param name="pos">The position of the voxel in world space</param>
        /// <returns></returns>
        public byte GenerateVoxelType(Vector3Int pos)
        {
            int posX = pos.x;
            int posY = pos.y;
            int posZ = pos.z;

            // TERRAIN GENERATION CODE

            int height = (int)(Mathf.PerlinNoise(posX * .1f, posZ * .1f) * 3 + 13); // Noise method

            byte val = 2; // By default type 2

            if (posY > height) val = 0; // Greater than height, should be air
            if (posY == height) val = 1; // Equal to the height, make it grass

            return val;
        }


        /// <summary>
        /// Retrieves the current voxel type byte value of a position
        /// </summary>
        /// <param name="pos">The position of the voxel</param>
        /// <returns></returns>
        public byte GetVoxelValue(Vector3Int pos)
        {

            int posX = pos.x;
            int posY = pos.y;
            int posZ = pos.z;

            if (posX < 0 || posX > worldSize.x * chunkXSize - 1 || posY < 0 || posY > chunkYSize - 1 || posZ < 0 || posZ > worldSize.y * chunkZSize - 1)
                return 0;

            //return GenerateVoxelType(new int3(posX, posY, posZ));

            ChunkCoord coord = GetChunkCoord(pos);


            Chunk foundChunk = chunks[coord.x, coord.z];

            // Chunk is already loaded
            if (foundChunk != null && foundChunk.generated)
            {
                return foundChunk.GetVoxelData(posX - (coord.x * chunkXSize), posY, posZ - (coord.z * chunkZSize)); // Return the byte value in the chunk to avoid extra noise call
            }

            return GenerateVoxelType(pos); // Chunk hasn't been generated yet, just generate the voxel

        }

        /// <summary>
        /// Retrieves the <see cref="VoxelType"/> of a position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public VoxelType GetVoxelType(Vector3Int pos)
        {
            return voxelTypes[GetVoxelValue(pos)];
        }
    }

    /// <summary>
    /// Represents a block's ID and transparency, as well as texturing
    /// </summary>
    [System.Serializable]
    public class VoxelType
    {
        [Header("Block Properties")]
        [Tooltip("The name of the block")]
        public string id;
        [Tooltip("Determines whether the block is air or not")]
        public bool isSolid;
    }
}