using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Realtin.Xdsl.Utilities;

internal static class AttributeUtility
{
	public static TAttribute? GetAttribute<TAttribute>(this MemberInfo element) where TAttribute : Attribute
	{
		return element.GetCustomAttribute<TAttribute>();
	}

	public static TValue GetAttributeValue<TAttribute, TValue>(this MemberInfo element,
		Func<TAttribute, TValue> getter, TValue defaultValue) where TAttribute : Attribute
	{
		var attribute = element.GetCustomAttribute<TAttribute>();

		if (attribute != null) {
			return getter(attribute);
		}

		return defaultValue;
	}

	public static bool TryGetAttribute<TAttribute>(this MemberInfo element, [NotNullWhen(true)] out TAttribute? attribute) where TAttribute : Attribute
	{
		attribute = GetAttribute<TAttribute>(element);

		return attribute is not null;
	}

    public static bool HasAttribute(this MemberInfo element, Type type) => Attribute.IsDefined(element, type);

    public static bool HasAttribute<TAttribute>(this MemberInfo element) where TAttribute : Attribute 
		=> Attribute.IsDefined(element, typeof(TAttribute));
}