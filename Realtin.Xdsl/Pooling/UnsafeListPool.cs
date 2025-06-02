using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Pooling;

internal static class UnsafeListPool<T>
{
	private static readonly object @lock = new();

	private static readonly Stack<List<T>> _pool = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<T> Acquire()
	{
		lock (@lock) {
			if (!_pool.TryPop(out List<T> list)) {
				list = [];
			}

			return list;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Release(List<T> list)
	{
		lock (@lock) {
			list.Clear();

			_pool.Push(list);
		}
	}
}