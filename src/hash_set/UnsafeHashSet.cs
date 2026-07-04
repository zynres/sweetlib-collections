using System.Runtime.InteropServices;

namespace unsafe_maps.src.hash_set;

public unsafe struct UnsafeHashSet<T> : IDisposable where T : unmanaged
{
    public uint?* Bucket;
    public Slot<T>* Slot;

    private uint bucketCapacity;
    private uint division;

    public uint Lenght;
    public uint Capacity;

    public UnsafeHashSet(uint capacity, uint division = 2)
    {
        this.division = division;

        Capacity = capacity;
        bucketCapacity = capacity / division;

        Slot = (Slot<T>*)NativeMemory.Alloc((nuint)(sizeof(Slot<T>) * capacity));
        Bucket = (uint?*)NativeMemory.Alloc(sizeof(uint) * bucketCapacity);

        NativeMemory.Clear(Slot, (nuint)sizeof(Slot<T>) * capacity);
        NativeMemory.Clear(Bucket, sizeof(int) * bucketCapacity);
    }
}