using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.NamingConventions;

namespace Realtin.Xdsl.Serialization;

/// <summary>
/// Provides options for XDSL serialization.
/// </summary>
public sealed class XdslSerializerOptions : ICloneable<XdslSerializerOptions>
{
	/// <summary>
	/// Default options for XDSL.
	/// </summary>
	public static XdslSerializerOptions Default { get; } = new XdslSerializerOptions() {
		IgnoreNonPublicFields = true,
		SerializeProperties = true,
		SelfReferenceHandling = SelfReferenceHandling.Ignore,
		EnumHandling = EnumHandling.Number,
		OptimizationLevel = XdslOptimizationLevel.Low
	};

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private INamingConvention _namingConvention = new PascalCaseNamingConvention();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ITypeFinder _typeFinder = new DefaultTypeFinder();

	/// <summary>
	/// Returns a value that indicates whether non public members should be serialized.
	/// </summary>
	public bool IgnoreNonPublicFields { get; set; }

	/// <summary>
	/// Returns a value that indicates whether properties should be serialized.
	/// </summary>
	public bool SerializeProperties { get; set; }
	
	/// <summary>
	/// Returns a value that indicates whether members of 
	/// a class must have the <see cref="XdslSerializeAttribute"/> to be serialized.
	/// </summary>
	public bool ExplicitSerialization { get; set; }

	public SelfReferenceHandling SelfReferenceHandling { get; set; }

	public EnumHandling EnumHandling { get; set; }

	public XdslOptimizationLevel OptimizationLevel { get; set; }

	public INamingConvention NamingConvention
	{
		get => _namingConvention;
		set => _namingConvention = value ?? throw new ArgumentNullException("NamingConvention");
	}

	public ITypeFinder TypeFinder
	{
		get => _typeFinder;
		set => _typeFinder = value ?? throw new ArgumentNullException(nameof(TypeFinder));
	}

	public XdslConverterCollection Converters { get; } = [];

	public XdslSerializerCollection Serializers { get; } = [];

	/// <summary>
	/// Gets or Sets a custom <see langword="FieldInfo"/> filter.
	/// </summary>
	public Func<FieldInfo, XdslSerializerOptions, bool>? FieldFilter { get; set; }

	/// <summary>
	/// Gets or Sets a custom <see langword="PropertyInfo"/> filter.
	/// </summary>
	public Func<PropertyInfo, XdslSerializerOptions, bool>? PropertyFilter { get; set; }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetConverter(Type type, [NotNullWhen(true)] out IXdslConverter? converter)
	{
		int length = Converters.Count;
		for (int i = 0; i < length; i++) {
			var converter1 = Converters[i];

			if (converter1.CanSerialize(type)) {
				converter = converter1;

				return true;
			}
		}

		converter = null;

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetConverter<T>([NotNullWhen(true)] out XdslConverter<T>? converter)
	{
		int length = Converters.Count;
		for (int i = 0; i < length; i++) {
			var converter1 = Converters[i];

			if (converter1.CanSerialize(typeof(T))) {
				converter = (XdslConverter<T>)converter1;

				return true;
			}
		}

		converter = null;

		return false;
	}

	public XdslSerializerOptions Clone()
	{
		var clone = new XdslSerializerOptions() {
			IgnoreNonPublicFields = IgnoreNonPublicFields,
			SerializeProperties = SerializeProperties,
			SelfReferenceHandling = SelfReferenceHandling,
			EnumHandling = EnumHandling,
			NamingConvention = NamingConvention,
			OptimizationLevel = OptimizationLevel,
			TypeFinder = TypeFinder,
			FieldFilter = FieldFilter,
			PropertyFilter = PropertyFilter,
		};

		//clone.Activators.AddRange(Activators);
		clone.Serializers.AddRange(Serializers);
		clone.Converters.AddRange(Converters);

		return clone;
	}

	public XdslSerializerOptions Clone(Action<XdslSerializerOptions> with)
	{
		var clone = Clone();

		with(clone);

		return clone;
	}
}