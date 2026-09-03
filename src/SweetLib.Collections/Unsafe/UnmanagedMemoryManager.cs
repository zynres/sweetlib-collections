using System.Buffers;

namespace SweetLib.Collections.Unsafe;

public unsafe sealed class UnmanagedMemoryManager<T>(T* pointer, int length) : MemoryManager<T> where T : unmanaged
{
    private readonly T* pointer = pointer;
    private readonly int length = length;

    public override Span<T> GetSpan()
        => new(pointer, length);

    public override MemoryHandle Pin(int elementIndex = 0)
        => new(pointer + elementIndex);

    public override void Unpin() { }

    protected override void Dispose(bool disposing) { }
}
