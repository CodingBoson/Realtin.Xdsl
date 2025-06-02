using System;
using System.Diagnostics.CodeAnalysis;
using Realtin.Xdsl.Buffers;

namespace Realtin.Xdsl.Text;

public sealed class XdslUnicodeEncoder : IXdslEncoder
{
    public string Encoding { get; } = "Unicode";

    [return: NotNullIfNotNull(nameof(input))]
    public string? Encode(string? input)
    {
        if (input == null) {
            return null;
        }

        var unicodeBytes = System.Text.Encoding.Unicode.GetBytes(input);

        return BitConverter.ToString(unicodeBytes);
    }

    [return: NotNullIfNotNull(nameof(input))]
    public string? Decode(string? input)
    {
        if (input == null) {
            return null;
        }

        ScopedBuffer<byte> scoped = XdslEncoding.ToBytesBuffered(input);

        var utf8Bytes = scoped.AsSpan();

        var text = System.Text.Encoding.Unicode.GetString(utf8Bytes);

        scoped.Dispose();

        return text;
    }
}