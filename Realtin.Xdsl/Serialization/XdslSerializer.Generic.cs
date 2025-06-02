using System;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Serialization;

public abstract class XdslSerializer<T> : XdslSerializer
{
	private string? _name;

	public virtual string GetXName() => _name ??= TypeUtility.LegalXNameFromType(typeof(T));

	public abstract void Serialize(XdslWriter writer, T? value, XdslSerializerOptions options);

	public abstract T? Deserialize(XdslReader reader, XdslSerializerOptions options);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool CanSerialize(Type type) => typeof(T) == type;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string GetXName(Type type) => GetXName();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Serialize(XdslWriter writer, object? value, XdslSerializerOptions options)
		=> Serialize(writer, (T?)value, options);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override object? Deserialize(XdslReader reader, Type type, XdslSerializerOptions options)
		=> Deserialize(reader, options);
}