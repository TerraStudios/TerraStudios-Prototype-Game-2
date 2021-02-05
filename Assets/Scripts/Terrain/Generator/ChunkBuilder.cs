//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration
{
    public class ChunkBuilder
    {
        private Chunk chunk;
        private TerrainGenerator generator;

        public ChunkBuilder(Chunk chunk)
        {
            this.chunk = chunk;

            generator = chunk.generator;
        }

        /// <summary>
        /// Handles job creation and destruction for the noise of a chunk.
        /// </summary>
        public class ChunkNoiseHandler
        {
            private int chunkXSize;
            private int chunkYSize;
            private int chunkZSize;

            private NativeArray<byte> nativeVoxelData;

            public ChunkNoiseHandler(int chunkXSize, int chunkYSize, int chunkZSize)
            {
                this.chunkXSize = chunkXSize;
                this.chunkYSize = chunkYSize;
                this.chunkZSize = chunkZSize;
            }

            /// <summary>
            /// Disposes all native collections created for jobs
            /// </summary>
            public void DisposeNatives()
            {
                if (nativeVoxelData.IsCreated)
                {
                    nativeVoxelData.Dispose();
                }
            }

            ~ChunkNoiseHandler()
            {
                jobHandle.Complete();
                DisposeNatives();
            }

            public JobHandle jobHandle;

            public IEnumerator StartNoiseJob(byte[] voxelData)
            {
                int voxelCount = chunkXSize * chunkYSize * chunkZSize;

                nativeVoxelData = new NativeArray<byte>(voxelCount, Allocator.TempJob);

                ChunkNoiseJob chunkNoiseJob = new ChunkNoiseJob
                {
                    voxelData = nativeVoxelData,
                    chunkSizeX = chunkXSize,
                    chunkSizeY = chunkYSize,
                    chunkSizeZ = chunkZSize,
                    x = 0,
                    y = 0,
                    z = 0
                };

                jobHandle = chunkNoiseJob.Schedule(voxelCount, 64);

                int frameCount = 0;

                yield return new WaitUntil(() =>
                {
                    frameCount++;
                    return jobHandle.IsCompleted || frameCount >= 4;
                });

                jobHandle.Complete();

                nativeVoxelData.CopyTo(voxelData);

                yield return null;
            }
        }

        public class ChunkMeshHandler
        {
            public Mesh chunkMesh;

            private NativeArray<byte> nativeVoxelData;
            private NativeArray<VoxelType> nativeVoxelTypes;
            private NativeCounter counter;
            private NativeArray<Vector3> nativeVertices;
            private NativeArray<Vector2> nativeUvs;
            private NativeArray<int> nativeTriangles;

            private int chunkXSize;
            private int chunkYSize;
            private int chunkZSize;

            public ChunkMeshHandler(int chunkXSize, int chunkYSize, int chunkZSize)
            {
                this.chunkXSize = chunkXSize;
                this.chunkYSize = chunkYSize;
                this.chunkZSize = chunkZSize;
            }

            /// <summary>
            /// Disposes all native collections created for jobs
            /// </summary>
            public void DisposeNatives()
            {
                if (nativeVoxelData.IsCreated)
                {
                    nativeVoxelData.Dispose();
                }

                if (nativeVoxelTypes.IsCreated)
                {
                    nativeVoxelTypes.Dispose();
                }

                if (counter.IsCreated)
                {
                    counter.Dispose();
                }

                if (nativeVertices.IsCreated)
                {
                    nativeVertices.Dispose();
                }

                if (nativeUvs.IsCreated)
                {
                    nativeUvs.Dispose();
                }

                if (nativeTriangles.IsCreated)
                {
                    nativeTriangles.Dispose();
                }
            }

            /// <summary>
            /// This was required because of chunks unloaded before reaching the dispose section of <see cref="StartMeshJob(byte[])"/>.
            /// </summary>
            ~ChunkMeshHandler()
            {
                jobHandle.Complete();
                DisposeNatives();
            }

            public JobHandle jobHandle;

            public IEnumerator StartMeshJob(byte[] voxelData)
            {

                int voxelCount = chunkXSize * chunkYSize * chunkZSize;

                nativeVoxelData = new NativeArray<byte>(voxelCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                nativeVoxelTypes = new NativeArray<VoxelType>(TerrainGenerator.instance.voxelTypes.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                counter = new NativeCounter(Allocator.TempJob);

                nativeVertices = new NativeArray<Vector3>(voxelCount * 12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                nativeUvs = new NativeArray<Vector2>(voxelCount * 12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                nativeTriangles = new NativeArray<int>(voxelCount * 18, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

                nativeVoxelData.CopyFrom(voxelData);
                nativeVoxelTypes.CopyFrom(TerrainGenerator.instance.voxelTypes);

                ChunkMeshJob chunkNoiseJob = new ChunkMeshJob
                {
                    voxelData = nativeVoxelData,
                    chunkSizeX = chunkXSize,
                    chunkSizeY = chunkYSize,
                    chunkSizeZ = chunkZSize,
                    voxelTypes = nativeVoxelTypes,
                    vertices = nativeVertices,
                    uvs = nativeUvs,
                    triangles = nativeTriangles,
                    counter = counter.ToConcurrent(),
                };

                jobHandle = chunkNoiseJob.Schedule(voxelCount, 64);
                JobHandle.ScheduleBatchedJobs();

                int frameCount = 0;

                yield return new WaitUntil(() =>
                {
                    frameCount++;
                    return jobHandle.IsCompleted || frameCount >= 4;
                });

                jobHandle.Complete();
                JobHandle.ScheduleBatchedJobs();

                chunkMesh = new Mesh();
                chunkMesh.SetVertices(nativeVertices, 0, counter.Count * 4);
                chunkMesh.SetUVs(0, nativeUvs, 0, counter.Count * 4);
                chunkMesh.SetIndices(nativeTriangles, 0, counter.Count * 6, MeshTopology.Triangles, 0);

                DisposeNatives();
            }

            public Mesh GetMeshData()
            {
                return chunkMesh;
            }
        }

        [BurstCompile]
        struct ChunkNoiseJob : IJobParallelFor
        {
            [ReadOnly] public int chunkSizeX;
            [ReadOnly] public int chunkSizeY;
            [ReadOnly] public int chunkSizeZ;

            [WriteOnly]
            public NativeArray<byte> voxelData;

            // Used for caching variables when calculating index
            // ! DivRem index lookup could possibly be a shared static method between the two jobs
            // ! This change would likely need to be benchmarked to make sure there is no impactful performance cost of doing so
            public int x;
            public int y;
            public int z;

            public void Execute(int i)
            {
                int zQ = Math.DivRem(i, chunkSizeZ, out z);
                int yQ = Math.DivRem(zQ, chunkSizeY, out y);
                x = yQ % chunkSizeX;

                // NOISE CODE
                // Note: This will be replaced by a better noise system in the future.

                voxelData[i] = 1;//TerrainGenerator.GenerateVoxelType(x, y, z); // Calculate noise
            }
        }

        [BurstCompile]
        struct ChunkMeshJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> voxelData;

            [ReadOnly]
            [NativeDisableParallelForRestriction]
            public NativeArray<VoxelType> voxelTypes;

            [WriteOnly]
            [NativeDisableParallelForRestriction]
            public NativeArray<Vector3> vertices;

            [WriteOnly]
            [NativeDisableParallelForRestriction]
            public NativeArray<Vector2> uvs;

            [WriteOnly]
            [NativeDisableParallelForRestriction]
            public NativeArray<int> triangles;

            [WriteOnly]
            public NativeCounter.Concurrent counter;

            [ReadOnly] public int chunkSizeX;
            [ReadOnly] public int chunkSizeY;
            [ReadOnly] public int chunkSizeZ;

            public void Execute(int i)
            {
                // Used for caching variables when calculating index
                int cx;
                int cy;
                int cz;

                int zQ = Math.DivRem(i, chunkSizeZ, out cz);
                int yQ = Math.DivRem(zQ, chunkSizeY, out cy);
                cx = yQ % chunkSizeX;

                int3 pos = new int3(cx, cy, cz);

                // If the block isn't solid don't try rendering it
                if (!voxelTypes[voxelData[i]].isSolid) return;
                //if (!CheckBlock(cubePos)) return;

                // Loop through every side of the voxel
                for (int p = 0; p < 6; p++)
                {
                    // Checks each side whether it contains a block, used for omitting sides that don't need to be rendered

                    if (!CheckBlock(i, pos + VoxelTables.faces[p]))
                    {
                        // 0, 0, 0 ; face 0 
                        // vert 1 - (0, 0, 0)
                        // vert 2 - (0, 1, 0)
                        // vert 3 - (1, 0, 0)
                        // vert 4 - (1, 1, 0)

                        // Add 4 vertices of cube side

                        float3 v1 = pos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p * 4]];
                        float3 v2 = pos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p * 4 + 1]];
                        float3 v3 = pos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p * 4 + 2]];
                        float3 v4 = pos + VoxelTables.voxelVerts[VoxelTables.voxelTris[p * 4 + 3]];

                        // (currentIndex * 24 [24 vertices in previous cube]) + (p [current face] * 4 [verts per face])
                        // (currentIndex * 36 [36 indices in previous cube]) + (p [current face] * 6 [indices per face])
                        int currentCount = counter.Increment();

                        int vIndex = (currentCount * 4);
                        int tIndex = (currentCount * 6);

                        vertices[vIndex] = (new Vector3(v1.x, v1.y, v1.z));
                        vertices[vIndex + 1] = (new Vector3(v2.x, v2.y, v2.z));
                        vertices[vIndex + 2] = (new Vector3(v3.x, v3.y, v3.z));
                        vertices[vIndex + 3] = (new Vector3(v4.x, v4.y, v4.z));

                        // Find x and y in relation to the texture 
                        float y = pos.z / (float)chunkSizeZ; //because uvs start top left, y needs to be inverted
                        float x = pos.x / (float)chunkSizeX;

                        float yAmount = 1f / chunkSizeZ;
                        float xAmount = 1f / chunkSizeX;

                        uvs[vIndex] = (new Vector2(x, y)); // 0, 0 - bottom left
                        uvs[vIndex + 1] = (new Vector2(x, y + yAmount)); //0, 1 - top left
                        uvs[vIndex + 2] = (new Vector2(x + xAmount, y)); // 1, 0 - bottom right
                        uvs[vIndex + 3] = (new Vector2(x + xAmount, y + yAmount)); // 1, 1 - top right

                        // Add triangles
                        triangles[tIndex] = (vIndex);
                        triangles[tIndex + 1] = (vIndex + 1);
                        triangles[tIndex + 2] = (vIndex + 2);

                        triangles[tIndex + 3] = (vIndex + 2);
                        triangles[tIndex + 4] = (vIndex + 1);
                        triangles[tIndex + 5] = (vIndex + 3);

                        // Signal another face has been created in the chunk, used for mesh data later on

                        // Increment index for next faces
                    }
                }
            }

            /// <summary>
            /// Checks whether a block is solid or not.
            /// Checks adjacent chunks if the block isn't inside this one
            /// </summary>
            /// <param name="pos">The position of the cube</param>
            /// <returns>True if the block is solid, false otherwise</returns>
            private bool CheckBlock(int i, int3 pos)
            {
                if (!VoxelInsideChunk(pos))
                {
                    return false;

                    //return generator.voxelTypes[generator.GetVoxelValue(pos + WorldPos)].isSolid;
                }

                return voxelTypes[voxelData[i]].isSolid;
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
        }
    }
}
