namespace unsafe_maps.src.hash_set;

public unsafe struct UnsafeHashSet<T> : IDisposable where T : unmanaged
{
    public uint?* Bucket;
    public Slot<T>* Slot;

    private uint bucketCapacity;
    private uint division;

    public uint Lenght;
    public uint Capacity;
}