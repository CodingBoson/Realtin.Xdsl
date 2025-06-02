using System;
using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl.Utilities;

internal readonly ref struct TypeSettings
{
	public readonly XdslSerializerOptions Options;

	public readonly XdslOverrideSettingsAttribute? SettingsAttribute;

	public readonly bool IgnoreNonPublicFields;

	public readonly bool SerializeProperties;

	public readonly bool ExplicitSerialization;

	public readonly SelfReferenceHandling SelfReferenceHandling;

	public readonly EnumHandling EnumHandling;

	public readonly XdslOptimizationLevel OptimizationLevel;

	public TypeSettings(Type type, XdslSerializerOptions options)
	{
		Options = options;

		if (type.TryGetAttribute(out SettingsAttribute)) {
			IgnoreNonPublicFields = SettingsAttribute.IgnoreNonPublicFields;
			SerializeProperties = SettingsAttribute.SerializeProperties;
			ExplicitSerialization = SettingsAttribute.ExplicitSerialization;
			SelfReferenceHandling = SettingsAttribute.SelfReferenceHandling;
			EnumHandling = SettingsAttribute.EnumHandling;
			OptimizationLevel = SettingsAttribute.OptimizationLevel;
		}
		else {
			IgnoreNonPublicFields = Options.IgnoreNonPublicFields;
			SerializeProperties = Options.SerializeProperties;
			ExplicitSerialization = Options.ExplicitSerialization;
			SelfReferenceHandling = Options.SelfReferenceHandling;
			EnumHandling = Options.EnumHandling;
			OptimizationLevel = Options.OptimizationLevel;
		}
	}
}