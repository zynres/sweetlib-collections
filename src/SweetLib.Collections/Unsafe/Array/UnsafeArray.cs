// Copyright © 2026 Zynres.

using SweetLib.Collections.Unsafe.List;
using System.Runtime.InteropServices;

namespace SweetLib.Collections.Unsafe.Array;

public unsafe struct UnsafeArray<T> where T : unmanaged
{
    public T* Data;

    public uint Length;

    public UnsafeArray(uint capacity)
    {
        Length = capacity;
        Data = (T*)NativeMemory.Alloc((nuint)(sizeof(T) * capacity));
    }

    public void Set(uint index, T value)
    {
        if (index >= Length)
            throw new IndexOutOfRangeException();

        *(Data + index) = value;
    }

    public readonly ref T Get(uint index)
    {
        if (index >= Length)
            throw new IndexOutOfRangeException();

        return ref Data[index];
    }

    public readonly ref T this[uint index] => ref Get(index);

    public void SetLength(uint length)
    {
        Length = length;
    }

    public readonly void CopyTo(UnsafeList<T>* map)
    {
        if (Data == null || map->Data == null)
            return;

        if (map->Capacity < Length)
            throw new Exception("Overflow");

        Buffer.MemoryCopy(
            Data, map->Data,
            map->Capacity * sizeof(T),
            Length * sizeof(T));

        map->Length = Length;
    }

    public readonly Span<T> AsSpan()
    {
        return new Span<T>(Data, (int)Length);
    }

    public void Dispose()
    {
        if (Data != null)
        {
            NativeMemory.Free(Data);
            Data = null;
        }

        Length = 0;
    }
}
