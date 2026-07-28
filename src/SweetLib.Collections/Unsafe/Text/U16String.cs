using System.Runtime.InteropServices;

namespace SweetLib.Collections.Unsafe.Text;

public unsafe struct U16String : IEquatable<U16String>
{
    public char* Data;
    public int Length;

    public U16String(in string text)
    {
        Length = text.Length;
        Data = (char*)NativeMemory.Alloc((nuint)(sizeof(char) * Length));

        fixed (char* src = text)
        {
            Buffer.MemoryCopy(
                src,
                Data,  
                Length * sizeof(char), 
                Length * sizeof(char));
        }
    }

    public static implicit operator U16String(in string text) => 
        new (in text);

    public override readonly bool Equals(object obj) =>
        obj is U16String other && Equals(other);

    public readonly bool Equals(U16String other)
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
        new(Data, 0, Length);

    public readonly ReadOnlySpan<char> AsSpan() =>
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

    public static bool operator ==(U16String left, U16String right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(U16String left, U16String right)
    {
        return !(left == right);
    }
}