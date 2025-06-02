using System;

namespace Realtin.Xdsl.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class XdslOverrideSettingsAttribute : Attribute
{
	/// <summary>
	/// Returns a value that indicates whether non public members should be serialized.
	/// </summary>
	public bool IgnoreNonPublicFields { get; set; } = true;

	/// <summary>
	/// Returns a value that indicates whether properties should be serialized.
	/// </summary>
	public bool SerializeProperties { get; set; } = true;

	/// <summary>
	/// Returns a value that indicates whether members of
	/// a class must have the <see cref="XdslSerializeAttribute"/> to be serialized.
	/// </summary>
	public bool ExplicitSerialization { get; set; }

	public SelfReferenceHandling SelfReferenceHandling { get; set; } = SelfReferenceHandling.Ignore;

	public EnumHandling EnumHandling { get; set; } = EnumHandling.Number;

	public XdslOptimizationLevel OptimizationLevel { get; set; } = XdslOptimizationLevel.None;
}