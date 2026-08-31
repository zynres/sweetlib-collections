// Copyright © 2026 Zynres.

using System.Runtime.InteropServices;

namespace SweetLib.Collections.Unsafe.Concurrent.Queue;

public unsafe struct UnsafeConcurrentQueue<T> where T : unmanaged
{
    public T* Data;

    public uint Capacity;
    public uint Length;

    public uint Write;
    public uint Read;

    private readonly Lock _sync;

    public UnsafeConcurrentQueue(uint capacity)
    {
        Read = 0;
        Write = 0;
        Length = 0;
        Capacity = Math.Max(2u, capacity);

        Data = (T*)NativeMemory.Alloc((nuint)(sizeof(T) * Capacity));

        _sync = new();
    }

    public void Enqueue(in T value)
    {
        lock (_sync)
        {
            if (Length == Capacity)
                Resize(Capacity * 2);

            Data[Write] = value;

            Write++;

            if (Write == Capacity)
                Write = 0;

            Length++;
        }
    }

    public bool TryDequeue(out T value)
    {
        lock (_sync)
        {
            if (Length == 0)
            {
                value = default;
                return false;
            }

            value = Data[Read];

            Read++;

            if (Read == Capacity)
                Read = 0;

            Length--;

            return true;
        }
    }

    public void Clear()
    {
        Read = 0;
        Write = 0;
        Length = 0;
    }

    private void Resize(uint newCapacity)
    {
        T* newData = (T*)NativeMemory.Alloc((nuint)(newCapacity * sizeof(T)));

        Write = Capacity;

        Buffer.MemoryCopy(
            Data, newData,
            newCapacity * sizeof(T),
            Capacity * sizeof(T));

        NativeMemory.Free(Data);

        Capacity = newCapacity;
        Data = newData;
    }

    public void Dispose()
    {
        if (Data != null)
        {
            NativeMemory.Free(Data);
            Data = null;
        }

        Capacity = 0;
    }
}
