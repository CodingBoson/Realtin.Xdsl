using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace Realtin.Xdsl.Pooling;

internal static class StringBuilderPool
{
	private static readonly ConcurrentStack<StringBuilder> _free = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder Rent()
	{
		if (!_free.TryPop(out StringBuilder pooled)) {
			pooled = new StringBuilder();
		}

		return pooled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Return(StringBuilder sb)
	{
		sb.Clear();

		_free.Push(sb);
	}
}