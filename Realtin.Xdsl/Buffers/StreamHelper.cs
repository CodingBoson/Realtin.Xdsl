using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Realtin.Xdsl.Buffers;

internal static class StreamHelper
{
	internal const int chunkSize = 1024;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] ToArray(Stream stream)
	{
		byte[] tempBuffer = ArrayPool<byte>.Shared.Rent(chunkSize);

		int position = 0;
		while (true) {
			var buffer = tempBuffer.AsSpan(position);
			int bytesRead = stream.Read(buffer);

			if (bytesRead < buffer.Length) {
				var data = tempBuffer.AsSpan(0, position + bytesRead).ToArray();

				ArrayPool<byte>.Shared.Return(tempBuffer, true);

				return data;
			}

			position += buffer.Length;

			tempBuffer = RentBiggerBuffer(tempBuffer, growth: chunkSize);
		}
	}

	public static async Task<byte[]> ToArrayAsync(Stream stream)
	{
		byte[] tempBuffer = ArrayPool<byte>.Shared.Rent(chunkSize);

		int position = 0;
		while (true) {
			var buffer = tempBuffer.AsMemory(position);
			int bytesRead = await stream.ReadAsync(buffer);

			if (bytesRead < buffer.Length) {
				var data = tempBuffer.AsSpan(0, position + bytesRead).ToArray();

				ArrayPool<byte>.Shared.Return(tempBuffer, true);

				return data;
			}

			position += buffer.Length;

			tempBuffer = RentBiggerBuffer(tempBuffer, chunkSize);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] RentBiggerBuffer(byte[] oldBuffer, int growth)
	{
		var buffer = ArrayPool<byte>.Shared.Rent(oldBuffer.Length + growth);

		oldBuffer.AsSpan().CopyTo(buffer);

		ArrayPool<byte>.Shared.Return(oldBuffer, true);

		return buffer;
	}
}