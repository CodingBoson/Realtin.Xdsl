using System;
using System.Diagnostics.CodeAnalysis;
using Realtin.Xdsl.Buffers;

namespace Realtin.Xdsl.Text;

public sealed class XdslUTF32Encoder : IXdslEncoder
{
    public string Encoding => "UTF-32";

    [return: NotNullIfNotNull(nameof(input))]
    public string? Encode(string? input)
    {
        if (input == null) {
            return null;
        }

        var utf32Bytes = System.Text.Encoding.UTF32.GetBytes(input);

        return BitConverter.ToString(utf32Bytes);
    }

    [return: NotNullIfNotNull(nameof(input))]
    public string? Decode(string? input)
    {
        if (input == null) {
            return null;
        }

        ScopedBuffer<byte> scoped = XdslEncoding.ToBytesBuffered(input);

        var utf8Bytes = scoped.AsSpan();

        var text = System.Text.Encoding.UTF32.GetString(utf8Bytes);

        scoped.Dispose();

        return text;
    }
}