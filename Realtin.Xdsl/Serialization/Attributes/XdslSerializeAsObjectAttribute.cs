using System;

namespace Realtin.Xdsl.Serialization;

/// <summary>
/// Indicates that the annotated collection should be serialized as an object.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class XdslSerializeAsObjectAttribute : XdslSerializerAttribute
{
	public XdslSerializeAsObjectAttribute()
	{ }
}