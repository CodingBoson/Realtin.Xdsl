using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Buffers;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Text;

public static class XdslEncoding
{
	private sealed class StringComparer : IEqualityComparer<string>
	{
		bool IEqualityComparer<string>.Equals(string x, string y)
		{
			return x.Equals(y, StringComparison.OrdinalIgnoreCase);
		}

		int IEqualityComparer<string>.GetHashCode(string obj)
		{
			return obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
		}
	}

	private static readonly Dictionary<string, IXdslEncoder> _encoders = new(new StringComparer());

	public static XdslUTF32Encoder UTF32Encoder { get; } = new XdslUTF32Encoder();

	public static XdslUTF8Encoder UTF8Encoder { get; } = new XdslUTF8Encoder();

	public static XdslUTF7Encoder UTF7Encoder { get; } = new XdslUTF7Encoder();

	public static XdslUnicodeEncoder UnicodeEncoder { get; } = new XdslUnicodeEncoder();

	static XdslEncoding()
	{
		RegisterEncoder(new XdslUTF32Encoder());
		RegisterEncoder(new XdslUTF8Encoder());
		RegisterEncoder(new XdslUTF7Encoder());
		RegisterEncoder(new XdslUnicodeEncoder());
	}

	public static void RegisterEncoder(IXdslEncoder encoder)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(encoder), encoder);

		_encoders.Add(encoder.Encoding, encoder);
	}

	public static IXdslEncoder GetEncoder(string encoding)
	{
		if (_encoders.TryGetValue(encoding, out var encoder)) {
			return encoder;
		}

		throw new XdslException($"Encoder for encoding '{encoding}' was not found.");
	}

    public static IXdslEncoder[] GetEncoders() => [.. _encoders.Values];

	public static IEnumerable<IXdslEncoder> EnumerateEncoders() => _encoders.Values;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] ToBytes(string s)
	{
		var bytesAsText = (ReadOnlySpan<char>)s;
		int length = bytesAsText.Length;
		using var bytes = new BufferWriter<byte>(length / 2);
		using var byteWriter = new BufferWriter<char>(4);

		for (int i = 0; i < length; i++) {
			char c = bytesAsText[i];

			if (c == '-') {
				var @byte = byte.Parse(byteWriter.AsSpan(), NumberStyles.HexNumber);

				bytes.Write(@byte);

				byteWriter.Reset();

				continue;
			}
			else if (i == length - 1) {
				byteWriter.Write(c);

				var @byte = byte.Parse(byteWriter.AsSpan(), NumberStyles.HexNumber);

				bytes.Write(@byte);
				byteWriter.Reset();

				continue;
			}

			byteWriter.Write(c);
		}

		return bytes.AsSpan().ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ScopedBuffer<byte> ToBytesBuffered(string s)
	{
		var bytesAsText = (ReadOnlySpan<char>)s;
		int length = bytesAsText.Length;
		var bytes = new BufferWriter<byte>(length / 2);
		using var byteWriter = new BufferWriter<char>(4);

		for (int i = 0; i < length; i++) {
			char c = bytesAsText[i];

			if (c == '-') {
				var @byte = byte.Parse(byteWriter.AsSpan(), NumberStyles.HexNumber);

				bytes.Write(@byte);
				byteWriter.Reset();

				continue;
			}
			else if (i == length - 1) {
				byteWriter.Write(c);

				var @byte = byte.Parse(byteWriter.AsSpan(), NumberStyles.HexNumber);

				bytes.Write(@byte);
				byteWriter.Reset();

				continue;
			}

			byteWriter.Write(c);
		}

		return new ScopedBuffer<byte>(bytes);
	}
}