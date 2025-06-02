using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

public sealed class XdslReader : IDisposable
{
	private readonly XdslNode _root;

	private int _propertyDepth;

	public XdslNode Current { get; private set; }

	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CanRead() ? _root.Children!.Count : 0;
	}

	public int CurrentLength
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (Current.Children is null)
				return 0;

			return Current.Children.Count;
		}
	}

	private XdslReader(XdslNode node)
	{
		_root = Current = node;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool CanRead()
	{
		return _root.Children is not null;
	}

	/// <exception cref="XdslException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslElement GetProperty(string propertyName, int depth = 0)
	{
		var children = Current.Children ?? throw new XdslException($"Property '{propertyName}' was not found.");

		for (int i = 0; i < children.Count; i++) {
			var property = children[i];

			if (property.Name == propertyName) {
				if (depth > 0) {
					depth--;

					continue;
				}

				return property;
			}
		}

		throw new XdslException($"Property '{propertyName}' was not found at depth {depth}.");
	}

	/// <exception cref="XdslException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslElement GetProperty(int index)
	{
		var children = Current.Children ?? throw new XdslException($"Property at {index} was not found.");

		if (index < 0 || index >= children.Count) {
			throw new ArgumentOutOfRangeException("index");
		}

		return children[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EnterProperty(string propertyName)
	{
		Current = GetProperty(propertyName);
		_propertyDepth++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ExitProperty()
	{
		if (_propertyDepth > 0) {
			Current = ((XdslElement)Current).Parent!;
			_propertyDepth--;

			return;
		}

		throw new InvalidOperationException("ExitProperty was called before EnterProperty.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBool(string propertyName)
	{
		return bool.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte ReadByte(string propertyName)
	{
		return byte.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte ReadSByte(string propertyName)
	{
		return sbyte.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short ReadShort(string propertyName)
	{
		return short.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ushort ReadUShort(string propertyName)
	{
		return ushort.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ReadInt(string propertyName)
	{
		return int.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadUInt(string propertyName)
	{
		return uint.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long ReadLong(string propertyName)
	{
		return long.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong ReadULong(string propertyName)
	{
		return ulong.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float ReadFloat(string propertyName)
	{
		return float.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double ReadDouble(string propertyName)
	{
		return double.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public decimal ReadDecimal(string propertyName)
	{
		return decimal.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public char ReadChar(string propertyName)
	{
		return char.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string? ReadString(string propertyName)
	{
		return GetProperty(propertyName).Text;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime ReadDateTime(string propertyName)
	{
		return DateTime.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTimeOffset ReadDateTimeOffset(string propertyName)
	{
		return DateTimeOffset.Parse(GetProperty(propertyName).Text);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Guid ReadGuid(string propertyName)
	{
		return Guid.Parse(GetProperty(propertyName).Text);
	}

	//Might Be Useful.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
	}

	public static XdslReader Create(XdslNode node)
	{
		if (node is XdslDocument document) {
			if (document.Root is null) {
				throw new XdslException("Document does not have a root element.");
			}

			return new XdslReader(document.Root!);
		}

		return new XdslReader(node);
	}
}