using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Parsers;

internal ref struct XdslAttributeReader(ReadOnlySpan<char> chars)
{
	private readonly ReadOnlySpan<char> _chars = chars;

	private readonly int _length = chars.Length;

	private int _charPosition = 0;

	private Attribute _current;

	public readonly Attribute Current => _current;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool CanRead()
	{
		return _length >= 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Read()
	{
		if (_charPosition >= _length) {
			_current = default;

			return false;
		}

		try {
			return ReadImpl();
		}
		catch {
			ThrowInvalidException();

			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool ReadImpl()
	{
		int equalsNum = _chars.IndexOf(_charPosition, '=');

		ReadOnlySpan<char> name = _chars.Slice(_charPosition, equalsNum);

		_charPosition += equalsNum + _chars.IndexOf(_charPosition + equalsNum, '"') + 1;

		int quoteNum = _chars.IndexOf(_charPosition, '"');

		ReadOnlySpan<char> value = _chars.Slice(_charPosition, quoteNum);

		_charPosition += quoteNum + 2;

		_current = new Attribute(name.TrimEnd(), value);

		return true;
	}

	[DoesNotReturn]
	private void ThrowInvalidException()
	{
		throw new XdslException($"Invalid Xdsl Attribute '{_chars.ToString()}'.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset()
	{
		_charPosition = 0;
		_current = default;
	}

	public readonly ref struct Attribute(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
	{
		public readonly ReadOnlySpan<char> Name = name;

		public readonly ReadOnlySpan<char> Value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out string name, out string value)
		{
			name = Name.ToString();
			value = Value.ToString();
		}
	}
}