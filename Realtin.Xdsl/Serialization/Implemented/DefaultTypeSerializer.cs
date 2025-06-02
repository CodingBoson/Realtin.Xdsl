using System;
using System.Collections.Generic;

namespace Realtin.Xdsl.Serialization;

internal static class DefaultTypeSerializer
{
	private static readonly HashSet<Type> _defaults = [
		typeof(bool),
		typeof(byte),
		typeof(sbyte),
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(decimal),
		typeof(string),
		typeof(char),
		typeof(DateTime),
		typeof(Guid),
		typeof(TimeSpan),
		typeof(DateTimeOffset),
	];

    public static bool CanSerialize(Type type) => _defaults.Contains(type) || type.IsEnum;

    public static string? Serialize(object? value, XdslSerializerOptions options)
	{
		if (value == null)
			return null;

		if (value is Enum e) {
			if (options.EnumHandling == EnumHandling.Number) {
				return Convert.ToInt32(e).ToString();
			}
			else {
				return e.ToString();
			}
		}

		return value.ToString();
	}

	public static object? Deserialize(Type type, string? value, XdslSerializerOptions options)
	{
		if (value is null)
			return null;
		else if (type == typeof(bool))
			return bool.Parse(value);
		else if (type == typeof(byte))
			return byte.Parse(value);
		else if (type == typeof(sbyte))
			return sbyte.Parse(value);
		else if (type == typeof(short))
			return short.Parse(value);
		else if (type == typeof(ushort))
			return ushort.Parse(value);
		else if (type == typeof(int))
			return int.Parse(value);
		else if (type == typeof(uint))
			return uint.Parse(value);
		else if (type == typeof(long))
			return long.Parse(value);
		else if (type == typeof(ulong))
			return ulong.Parse(value);
		else if (type == typeof(float))
			return float.Parse(value);
		else if (type == typeof(double))
			return double.Parse(value);
		else if (type == typeof(decimal))
			return decimal.Parse(value);
		else if (type == typeof(string))
			return value;
		else if (type == typeof(char))
			return char.Parse(value);
		else if (type == typeof(DateTime))
			return DateTime.Parse(value);
		else if (type == typeof(Guid))
			return Guid.Parse(value);
		else if (type == typeof(TimeSpan))
			return TimeSpan.Parse(value);
		else if (type == typeof(DateTimeOffset))
			return DateTimeOffset.Parse(value);
		else if (type.IsEnum)
			return Enum.Parse(type, value);

		throw new XdslSerializerException($"Cannot deserialize type '{type}'.");
	}
}