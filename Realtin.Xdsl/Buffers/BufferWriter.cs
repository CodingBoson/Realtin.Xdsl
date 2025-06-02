using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Buffers;

internal struct BufferWriter<T> : IDisposable
{
	private T[] _buffer;

	private int _index;

	private int _bufferSize;

	private bool _disposed;

	public readonly int BufferCapacity => _bufferSize;

	public BufferWriter(int bufferSize)
	{
		_buffer = ArrayPool<T>.Shared.Rent(bufferSize);
		_bufferSize = _buffer.Length;
		_index = 0;
		_disposed = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(T value)
	{
		AssertIsNotDisposed();

		if (_index >= _bufferSize) {
			Resize();
		}

		_buffer[_index++] = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Resize()
	{
		const int margin = 124;

		var temp = _buffer;

		_buffer = ArrayPool<T>.Shared.Rent(_bufferSize + margin);

		temp.AsSpan().CopyTo(_buffer);

		temp.AsSpan().Clear();

		ArrayPool<T>.Shared.Return(temp);

		_bufferSize = _buffer.Length;
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset() => _index = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		AssertIsNotDisposed();

		_disposed = true;

		_buffer.AsSpan(0, _index).Clear();

		ArrayPool<T>.Shared.Return(_buffer);
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() => new(_buffer, 0, _index);

    private readonly void AssertIsNotDisposed()
	{
		if (_disposed) {
			throw new ObjectDisposedException(ToString());
		}
	}
}