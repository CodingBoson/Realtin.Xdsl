using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Buffers;

public ref struct ScopedBuffer<T>
{
	private readonly BufferWriter<T> _writer;

	private Span<T> _span;

	internal ScopedBuffer(BufferWriter<T> writer) : this()
	{
		_writer = writer;
		_span = writer.AsSpan();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		_writer.Dispose();
		_span = default;
	}

	public readonly Span<T> AsSpan() => _span;
}