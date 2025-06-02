using System;
using System.Reflection;
using Realtin.Xdsl.Pooling;
using Realtin.Xdsl.Serialization;
using Realtin.Xdsl.Syntax;

namespace Realtin.Xdsl.Utilities;

internal static class TypeUtility
{
	#region Serialization

	private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static XdslPropertyInfo[] GetSerializationProperties(this Type type, XdslSerializerOptions options)
	{
		var list = UnsafeListPool<XdslPropertyInfo>.Acquire();

		var settings = new TypeSettings(type, options);
		var members = type.GetMembers(_flags);

		for (int i = 0; i < members.Length; i++) {
			var memberInfo = members[i];

			if (memberInfo is FieldInfo fieldInfo) {
				if (settings.ExplicitSerialization && !fieldInfo.HasAttribute<XdslSerializeAttribute>()) {
					continue;
				}

				if (CanSerialize(type, fieldInfo, settings)) {
					list.Add(new XdslPropertyInfo(fieldInfo, options));
				}
			}
			else if (memberInfo is PropertyInfo propertyInfo) {
				if (settings.ExplicitSerialization && !propertyInfo.HasAttribute<XdslSerializeAttribute>()) {
					continue;
				}

				if (CanSerialize(type, propertyInfo, settings)) {
					list.Add(new XdslPropertyInfo(propertyInfo, options));
				}
			}
		}

		var array = list.ToArray();

		UnsafeListPool<XdslPropertyInfo>.Release(list);

		return array;
	}

	private static bool CanSerialize(Type type, FieldInfo fieldInfo, TypeSettings settings)
	{
		var fieldType = fieldInfo.FieldType;

		if (fieldInfo.HasAttribute<XdslIgnoreAttribute>()) {
			return false;
		}

		if (fieldType == type) {
			switch (settings.SelfReferenceHandling) {
				case SelfReferenceHandling.Ignore:
					return false;

				case SelfReferenceHandling.Throw:
					throw new XdslException($"A self reference loop was detected for type {type} on {fieldInfo.Name}.");
				case SelfReferenceHandling.Serialize:
					break;
			}
		}

		if ((fieldType.IsAbstract || fieldType.IsInterface)
			&& !fieldInfo.HasAttribute<XdslSerializeReferenceAttribute>()) {
			return fieldInfo.HasAttribute<XdslSerializeAttribute>();
		}

		if (!fieldInfo.IsPublic) {
			if (settings.IgnoreNonPublicFields && !fieldInfo.HasAttribute<XdslSerializeAttribute>()) {
				return false;
			}
		}

		bool canWrite = !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;

		if (settings.Options.FieldFilter != null) {
			return canWrite && settings.Options.FieldFilter(fieldInfo, settings.Options);
		}

		return canWrite;
	}

	private static bool CanSerialize(Type type, PropertyInfo propertyInfo, TypeSettings settings)
	{
		var propertyType = propertyInfo.PropertyType;

		if (propertyInfo.HasAttribute<XdslIgnoreAttribute>()) {
			return false;
		}

		if (propertyType == type) {
			switch (settings.SelfReferenceHandling) {
				case SelfReferenceHandling.Ignore:
					return false;

				case SelfReferenceHandling.Throw:
					throw new XdslException($"A self reference loop was detected for type {type} on {propertyInfo.Name}.");
				case SelfReferenceHandling.Serialize:
					break;
			}
		}

		if ((propertyType.IsAbstract || propertyType.IsInterface)
			&& !propertyInfo.HasAttribute<XdslSerializeReferenceAttribute>()) {
			return propertyInfo.HasAttribute<XdslSerializeAttribute>();
		}

		if (!propertyInfo.IsPublic()) {
			if (settings.IgnoreNonPublicFields && !propertyInfo.HasAttribute<XdslSerializeAttribute>()) {
				return false;
			}
		}

		bool canReadWrite = propertyInfo.CanWrite && propertyInfo.CanRead;

		if (settings.Options.PropertyFilter != null) {
			return canReadWrite && settings.Options.PropertyFilter(propertyInfo, settings.Options);
		}

		// TODO: Improve indexer identifying.
		if (canReadWrite && propertyInfo.GetGetMethod(nonPublic: true).GetParameters().Length > 0) {
			return false;
		}

		return canReadWrite;
	}

	public static XdslPropertyType GetPropertyType(Type type)
	{
		if (type == typeof(bool)) {
			return XdslPropertyType.Boolean;
		}
		else if (type == typeof(byte)
			|| type == typeof(sbyte)
			|| type == typeof(short)
			|| type == typeof(ushort)
			|| type == typeof(int)
			|| type == typeof(uint)
			|| type == typeof(long)
			|| type == typeof(ulong)) {
			return XdslPropertyType.Integer;
		}
		else if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) {
			return XdslPropertyType.Float;
		}
		else if (type == typeof(string)) {
			return XdslPropertyType.String;
		}
		else if (type == typeof(char)) {
			return XdslPropertyType.Char;
		}
		else if (type == typeof(DateTime)) {
			return XdslPropertyType.DateTime;
		}
		else if (type == typeof(Guid)) {
			return XdslPropertyType.Guid;
		}
		else if (type.IsEnum) {
			return XdslPropertyType.Enum;
		}
		else {
			return XdslPropertyType.Object;
		}
	}

    #endregion Serialization

    #region Extensions

    public static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

    public static bool IsNullable(this Type type) 
		=> type.IsReferenceType() || Nullable.GetUnderlyingType(type) != null;

    public static bool IsReferenceType(this Type type) => !type.IsValueType;

    public static bool IsStruct(this Type type) => type.IsValueType && !type.IsPrimitive && !type.IsEnum;

    public static bool IsAssignableFrom(this Type type, object value)
	{
		if (value == null) {
			return type.IsNullable();
		}
		else {
			return type.IsInstanceOfType(value);
		}
	}

	#endregion Extensions

	#region Legal Names

	public static string LegalXdslName(this Type type, bool fullName)
	{
		if (type.TryGetAttribute<XdslNameAttribute>(out var xdslName)) {
			return xdslName.Name;
		}

		var typeName = fullName ? type.FullName : type.Name;

		return SyntaxHelper.LegalNameFast(typeName);
	}

    public static string LegalXNameFromType(Type type) => SyntaxHelper.LegalNameFast(type.Name);

    #endregion Legal Names
}