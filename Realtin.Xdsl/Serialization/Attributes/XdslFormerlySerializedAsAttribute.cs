using System;

namespace Realtin.Xdsl.Serialization;

public sealed class XdslFormerlySerializedAsAttribute(string name) : XdslSerializerAttribute
{
	public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
}