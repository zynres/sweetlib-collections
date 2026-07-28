using System.Text;

namespace SweetLib.Collections.Unsafe.Text;

public static class UTFExtension
{
    public static ReadOnlySpan<byte> ToU8(this string text)
    {
        return new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(text));
    }
}