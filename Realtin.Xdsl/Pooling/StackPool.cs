using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Pooling;

internal static class StackPool<T>
{
	private static readonly ConcurrentStack<Stack<T>> _free = [];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Stack<T> Rent()
	{
		if (_free.TryPop(out var stack)) {
			return stack;
		}

		return new Stack<T>();
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return(Stack<T> stack)
	{
		if (stack.Count > 0) {
			stack.Clear();
		}

		_free.Push(stack);
	}
}