using System.Runtime.InteropServices;
using System.Text;

namespace SweetLib.Collections.Unsafe.Text;

public unsafe struct U8String : IEquatable<U8String>
{
    public byte* Data;
    public int Length;

    public U8String(in ReadOnlySpan<byte> span)
    {
        Length = span.Length;
        Data = (byte*)NativeMemory.Alloc((nuint)Length);

        fixed (byte* src = span)
        {
            Buffer.MemoryCopy(
                src,
                Data,
                Length,
                Length);
        }
    }

    public U8String(in string text)
    {
        Length = Encoding.UTF8.GetByteCount(text);
        Data = (byte*)NativeMemory.Alloc((nuint)Length);

        fixed (char* src = text)
        {
            Buffer.MemoryCopy(
                src,
                Data,
                Length,
                Length);
        }
    }

    public static implicit operator U8String(in ReadOnlySpan<byte> span) =>
        new(in span);

    public static implicit operator U8String(in string text) =>
        new(in text);

    public static implicit operator U8String(in uint value) =>
        new(value.ToString());

    public static implicit operator U8String(in int value) =>
        new(value.ToString());

    public override readonly bool Equals(object obj) =>
        obj is U8String other && Equals(other);

    public readonly bool Equals(U8String other)
    {
        if (other.Length != Length)
            return false;

        for (int i = 0; i < Length; i++)
        {
            if (other.Data[i] != Data[i])
            {
                return false;
            }
        }

        return true;
    }

    public readonly override int GetHashCode()
    {
        unchecked
        {
            uint hash = 2166136261;

            for (int i = 0; i < Length; i++)
            {
                hash ^= Data[i];
                hash *= 16777619;
            }

            return (int)hash;
        }
    }

    public override readonly string ToString() =>
        Encoding.UTF8.GetString(Data, Length);

    public readonly ReadOnlySpan<byte> AsSpan() =>
        new(Data, Length);

    public void Dispose()
    {
        if (Data != null)
        {
            NativeMemory.Free(Data);
            Data = null;
            Length = 0;
        }
    }

    public static bool operator ==(U8String left, U8String right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(U8String left, U8String right)
    {
        return !(left == right);
    }

    public static U8String operator +(U8String left, U8String right)
    {
        int length = left.Length + right.Length;

        byte* data = (byte*)NativeMemory.Alloc((nuint)length);

        Buffer.MemoryCopy(left.Data, data, length, left.Length);

        Buffer.MemoryCopy(
            right.Data,
            data + left.Length,
            length - left.Length,
            right.Length);

        U8String result = default;
        result.Data = data;
        result.Length = length;

        return result;
    }
}
