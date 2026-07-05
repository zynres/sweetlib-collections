using System.Runtime.InteropServices;

namespace unsafe_maps.src.hash_set;

public unsafe struct UnsafeHashSet<T> : IDisposable where T : unmanaged
{
    public uint?* Bucket;
    public Slot<T>* Slot;

    private uint bucketCapacity;
    private readonly uint division;

    public uint Length;
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

    public void Add(in T value)
    {
        if (Length >= Capacity)
            Resize(Capacity * 2);

        int hash = value.GetHashCode();
        uint bucket_index = (uint)hash % bucketCapacity;
        uint?* bucket = &Bucket[bucket_index];

        Slot<T>* slot = &Slot[Length];

        slot->Value = value;
        slot->Hash = hash;
        slot->Next = *bucket;

        *bucket = Length;
        Length++;
    }

    public void Set(uint index, in T value)
    {
        if (index >= Length)
            throw new IndexOutOfRangeException();

        int hash = value.GetHashCode();

        uint bucket_index = (uint)hash % bucketCapacity;

        uint?* bucket = &Bucket[bucket_index];

        Slot<T>* slot = &Slot[index];

        slot->Next = *bucket;
        slot->Value = value;
        slot->Hash = hash;

        *bucket = index;
    }

    public readonly ref T Get(uint index)
    {
        if (index >= Length)
            throw new IndexOutOfRangeException();

        return ref Slot[index].Value;
    }

    public ref T Emplace(uint index)
    {
        if (index < Capacity && index >= Lenght)
            Lenght = index + 1;

        return ref Slot[index].Value;
    }

    public readonly bool Constaint(in T value)
    {
        int hash = value.GetHashCode();
        uint bucket_index = (uint)hash % bucketCapacity;

        uint?* index = &Bucket[bucket_index];

        while(true)
        {
            if (*index == null)
                return false;

            Slot<T>* slot = &Slot[index->Value];

            if (slot->Value.Equals(value))
                return true;

            index = &slot->Next;
        }
    }
    
    public readonly ref T this[uint index] => ref Get(index);
}