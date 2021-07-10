using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

[StructLayout(LayoutKind.Sequential)]
[NativeContainer]
public unsafe struct NativeCounter
{
    // The actual pointer to the allocated count needs to have restrictions relaxed so jobs can be schedled with this container
    [NativeDisableUnsafePtrRestriction]
    int* counter;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    AtomicSafetyHandle safety;
    // The dispose sentinel tracks memory leaks. It is a managed type so it is cleared to null when scheduling a job
    // The job cannot dispose the container, and no one else can dispose it until the job has run so it is ok to not pass it along
    // This attribute is required, without it this native container cannot be passed to a job since that would give the job access to a managed object
    [NativeSetClassTypeToNullOnSchedule]
    DisposeSentinel disposeSentinel;
#endif

    // Keep track of where the memory for this was allocated
    Allocator allocatorLabel;

    public NativeCounter(Allocator label)
    {
        // This check is redundant since we always use an int which is blittable.
        // It is here as an example of how to check for type correctness for generic types.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        if (!UnsafeUtility.IsBlittable<int>())
            throw new ArgumentException(string.Format("{0} used in NativeQueue<{0}> must be blittable", typeof(int)));
#endif
        allocatorLabel = label;

        // Allocate native memory for a single integer
        counter = (int*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<int>(), 4, label);

        // Create a dispose sentinel to track memory leaks. This also creates the AtomicSafetyHandle
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_2018_3_OR_NEWER
        DisposeSentinel.Create(out safety, out disposeSentinel, 0, label);
#else
        DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 0);
#endif
#endif
        // Initialize the count to 0 to avoid uninitialized data
        Count = 0;
    }

    public int Increment()
    {
        // Verify that the caller has write permission on this data.
        // This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(safety);
#endif
        (*counter)++;
        return (*counter) - 1;
    }

    public int Count
    {
        get
        {
            // Verify that the caller has read permission on this data.
            // This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(safety);
#endif
            return *counter;
        }
        set
        {
            // Verify that the caller has write permission on this data. This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(safety);
#endif
            *counter = value;
        }
    }

    public bool IsCreated
    {
        get { return counter != null; }
    }

    public void Dispose()
    {
        // Let the dispose sentinel know that the data has been freed so it does not report any memory leaks
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_2018_3_OR_NEWER
        DisposeSentinel.Dispose(ref safety, ref disposeSentinel);
#else
        DisposeSentinel.Dispose(m_Safety, ref m_DisposeSentinel);
#endif
#endif

        UnsafeUtility.Free(counter, allocatorLabel);
        counter = null;
    }

    public Concurrent ToConcurrent()
    {
        Concurrent concurrent;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(safety);
        concurrent.m_Safety = safety;
        AtomicSafetyHandle.UseSecondaryVersion(ref concurrent.m_Safety);
#endif

        concurrent.counter = counter;
        return concurrent;
    }

    [NativeContainer]
    // This attribute is what makes it possible to use NativeCounter.Concurrent in a ParallelFor job
    [NativeContainerIsAtomicWriteOnly]
    unsafe public struct Concurrent
    {
        // Copy of the pointer from the full NativeCounter
        [NativeDisableUnsafePtrRestriction]
        internal int* counter;

        // Copy of the AtomicSafetyHandle from the full NativeCounter. The dispose sentinel is not copied since this inner struct does not own the memory and is not responsible for freeing it
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#pragma warning disable IDE1006 // Naming Styles
        internal AtomicSafetyHandle m_Safety;
#pragma warning restore IDE1006 // Naming Styles
#endif

        public int Increment()
        {
            // Increment still needs to check for write permissions
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            // The actual increment is implemented with an atomic since it can be incremented by multiple threads at the same time
            return Interlocked.Increment(ref *counter) - 1;
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
[NativeContainer]
public unsafe struct NativePerThreadCounter
{
    // The actual pointer to the allocated count needs to have restrictions relaxed so jobs can be schedled with this container
    [NativeDisableUnsafePtrRestriction]
    int* counter;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    AtomicSafetyHandle safety;
    // The dispose sentinel tracks memory leaks. It is a managed type so it is cleared to null when scheduling a job
    // The job cannot dispose the container, and no one else can dispose it until the job has run so it is ok to not pass it along
    // This attribute is required, without it this native container cannot be passed to a job since that would give the job access to a managed object
    [NativeSetClassTypeToNullOnSchedule]
    DisposeSentinel disposeSentinel;
#endif

    // Keep track of where the memory for this was allocated
    Allocator allocatorLabel;

    public const int IntsPerCacheLine = JobsUtility.CacheLineSize / sizeof(int);

    public NativePerThreadCounter(Allocator label)
    {
        // This check is redundant since we always use an int which is blittable.
        // It is here as an example of how to check for type correctness for generic types.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        if (!UnsafeUtility.IsBlittable<int>())
            throw new ArgumentException(string.Format("{0} used in NativeQueue<{0}> must be blittable", typeof(int)));
#endif
        allocatorLabel = label;

        // One full cache line (integers per cacheline * size of integer) for each potential worker index, JobsUtility.MaxJobThreadCount
        counter = (int*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<int>() * IntsPerCacheLine * JobsUtility.MaxJobThreadCount, 4, label);

        // Create a dispose sentinel to track memory leaks. This also creates the AtomicSafetyHandle
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_2018_3_OR_NEWER
        DisposeSentinel.Create(out safety, out disposeSentinel, 0, label);
#else
        DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 0);
#endif
#endif
        // Initialize the count to 0 to avoid uninitialized data
        Count = 0;
    }

    public void Increment()
    {
        // Verify that the caller has write permission on this data.
        // This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(safety);
#endif
        (*counter)++;
    }

    public int Count
    {
        get
        {
            // Verify that the caller has read permission on this data.
            // This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(safety);
#endif
            int count = 0;
            for (int i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
                count += counter[IntsPerCacheLine * i];
            return count;
        }
        set
        {
            // Verify that the caller has write permission on this data.
            // This is the race condition protection, without these checks the AtomicSafetyHandle is useless
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(safety);
#endif
            // Clear all locally cached counts,
            // set the first one to the required value
            for (int i = 1; i < JobsUtility.MaxJobThreadCount; ++i)
                counter[IntsPerCacheLine * i] = 0;
            *counter = value;
        }
    }

    public bool IsCreated
    {
        get { return counter != null; }
    }

    public void Dispose()
    {
        // Let the dispose sentinel know that the data has been freed so it does not report any memory leaks
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_2018_3_OR_NEWER
        DisposeSentinel.Dispose(ref safety, ref disposeSentinel);
#else
        DisposeSentinel.Dispose(m_Safety, ref m_DisposeSentinel);
#endif
#endif

        UnsafeUtility.Free(counter, allocatorLabel);
        counter = null;
    }

    public Concurrent ToConcurrent()
    {
        Concurrent concurrent;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.CheckWriteAndThrow(safety);
        concurrent.safety = safety;
        AtomicSafetyHandle.UseSecondaryVersion(ref concurrent.safety);
#endif

        concurrent.counter = counter;
        concurrent.threadIndex = 0;
        return concurrent;
    }

    [NativeContainer]
    [NativeContainerIsAtomicWriteOnly]
    // Let the JobSystem know that it should inject the current worker index into this container
    unsafe public struct Concurrent
    {
        [NativeDisableUnsafePtrRestriction]
        internal int* counter;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle safety;
#endif

        // The current worker thread index, it must use this exact name since it is injected
        [NativeSetThreadIndex]
        internal int threadIndex;

        public void Increment()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(safety);
#endif
            // No need for atomics any more since we are just incrementing the local count
            ++counter[IntsPerCacheLine * threadIndex];
        }
    }
}
