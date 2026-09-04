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
    public uint Save;

    private readonly Lock _sync;

    public UnsafeConcurrentQueue(uint capacity)
    {
        Capacity = Math.Max(2u, capacity);
        Length = 0;
        Write = 0;
        Read = 0;
        Save = 0;

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

    public bool TryInQueue(out T value)
    {
        lock (_sync)
        {
            if (Save == Write)
            {
                value = default;
                return false;
            }

            value = Data[Save];

            Save++;

            if (Save == Capacity)
                Save = 0;

            return true;
        }
    }

    public void DeleteSaved(uint index)
    {
        lock (_sync)
        {
            index++;

            uint count;

            if (index >= Read)
                count = index - Read;
            else
                count = Capacity - Read + index;

            Read = index;
            Length -= count;
        }
    }

    public void SetReadLength(uint index, uint count) 
    {
        lock (_sync) 
        {
            Read = index;
            Length -= Length;
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
