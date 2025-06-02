using System;
using System.Diagnostics.CodeAnalysis;
using Realtin.Xdsl.Buffers;

namespace Realtin.Xdsl.Text;

public sealed class XdslUTF8Encoder : IXdslEncoder
{
    public string Encoding => "UTF-8";

    [return: NotNullIfNotNull(nameof(input))]
    public string? Encode(string? input)
    {
        if (input == null) {
            return null;
        }

        var utf8Bytes = System.Text.Encoding.UTF8.GetBytes(input);

        return BitConverter.ToString(utf8Bytes);
    }

    [return: NotNullIfNotNull(nameof(input))]
    public string? Decode(string? input)
    {
        if (input == null) {
            return null;
        }

        ScopedBuffer<byte> scoped = XdslEncoding.ToBytesBuffered(input);

        //Get a span from the buffer to avoid allocating a new array.
        var utf8Bytes = scoped.AsSpan();

        var text = System.Text.Encoding.UTF8.GetString(utf8Bytes);

        scoped.Dispose();

        return text;
    }
}