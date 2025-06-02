using System;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

internal static partial class ObjectDefaultSerializer
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Type GetPropertyType(object target, XdslPropertyInfo propertyInfo)
	{
		if (propertyInfo.SerializeByRef && target is IXdslSerializeReferenceResolver resolver) {
			return resolver.GetType(propertyInfo.Name)
				?? throw new XdslSerializerException($"ReferenceResolver {resolver.GetType()} returned a null type.");
		}

		return propertyInfo.Type;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement? GetProperty(XdslElement xdslObject, XdslPropertyInfo propertyInfo, XdslSerializerOptions options)
	{
		var xdslProperty = xdslObject.
			GetChild(options.NamingConvention.
			Apply(propertyInfo.Name));

		if (xdslProperty is null &&
			!string.IsNullOrEmpty(propertyInfo.FormerlySerializedAs)) {
			xdslProperty = xdslObject.
				GetChild(options.NamingConvention.
				Apply(propertyInfo.FormerlySerializedAs));
		}

		return xdslProperty;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement EmptyCollection()
	{
		var element = new XdslElement("");

		element.AddAttribute("null", "false");

		return element;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XdslElement NullElement(string name = "Object")
	{
		return new XdslElement(name);
	}

	[Obsolete("Use direct call instead.", true)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object CreateInstance(XdslTypeInfo typeInfo, XdslSerializerOptions options)
	{
		//var type = typeInfo.Type;

		//if (options.TryGetActivator(type, out var activator)) {
		//	return activator.CreateInstance(type, options);
		//}

		return typeInfo.Activator.CreateInstance();
	}
}