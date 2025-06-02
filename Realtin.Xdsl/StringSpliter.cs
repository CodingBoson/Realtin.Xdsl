using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl;

/// <summary>
/// Provides functionality to split a <see cref="ReadOnlySpan{T}"/> of characters.
/// </summary>
/// <param name="text"></param>
public ref struct StringSplitter(ReadOnlySpan<char> text)
{
	private readonly ReadOnlySpan<char> _chars = text;

	private readonly int _length = text.Length;

	private int _position = 0;

	/// <summary>
	/// The current position of this <see cref="StringSplitter"/>.
	/// </summary>
	public readonly int Position { get => _position; }

	/// <summary>
	/// The span owned by this <see cref="StringSplitter"/>.
	/// </summary>
	public readonly ReadOnlySpan<char> Span => _chars;

    /// <summary>
    /// Can this <see cref="StringSplitter"/> split?
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool CanSplit() => _position < _length;

    /// <summary>
    /// </summary>
    /// <param name="c"></param>
    /// <param name="segment"></param>
    /// <param name="trimEntries"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TrySplit(char c, out ReadOnlySpan<char> segment, bool trimEntries = false)
	{
		segment = default;

		if (!CanSplit()) {
			return false;
		}

		int num = _position;
		for (int i = num; i < _length; i++) {
			var ci = _chars[i];

			if (ci == c) {
				segment = trimEntries ? _chars[num.._position].Trim() : _chars[num.._position];

				_position++;

				return true;
			}
			else if (i == _length - 1) {
				_position++;

				segment = trimEntries ? _chars[num.._position].Trim() : _chars[num.._position];

				return true;
			}

			_position++;
		}

		return true;
	}
}