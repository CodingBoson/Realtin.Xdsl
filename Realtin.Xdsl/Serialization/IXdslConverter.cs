using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

public interface IXdslConverter
{
	bool CanSerialize(Type type);

	void Serialize(XdslValue xdslObject, object? value);

	object? Deserialize(XdslValue xdslObject, Type type);
}

public abstract class XdslConverter<T> : IXdslConverter
{
	public abstract void Serialize(XdslValue xdslValue, T? value);

	public abstract T? Deserialize(XdslValue xdslValue);

	public virtual bool CanSerialize(Type type) => type == typeof(T);

	void IXdslConverter.Serialize(XdslValue xdslValue, object? value) => Serialize(xdslValue, (T?)value);

	object? IXdslConverter.Deserialize(XdslValue xdslValue, Type type) => Deserialize(xdslValue);
}

public readonly struct XdslValue(XdslElement element)
{
	public XdslElement Element { get; } = element ?? throw new ArgumentNullException(nameof(element));

	public string? Value { get => Element.Text; set => Element.Text = value; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsEmpty() => string.IsNullOrWhiteSpace(Value);
}