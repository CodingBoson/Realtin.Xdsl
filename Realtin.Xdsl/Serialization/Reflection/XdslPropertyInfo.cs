using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Optimization;
using Realtin.Xdsl.Serialization;
using Realtin.Xdsl.Text;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

public sealed class XdslPropertyInfo
{
	private readonly IOptimizedAccessor? _accessor;

	private readonly bool _isEscaped;

	public string Name { get; }

	public string? FormerlySerializedAs { get; }

	public bool SerializeByRef { get; }

	public string? Encoding { get; }

	public bool IsRequired { get; set; }

	public XdslPropertyType PropertyType { get; }

	public Type Type { get; }

	public MemberInfo UnderlyingMember { get; }

	internal XdslPropertyInfo(MemberInfo memberInfo, XdslSerializerOptions options)
	{
		if (memberInfo is FieldInfo fieldInfo) {
			Type = fieldInfo.FieldType;

			if (options.OptimizationLevel >= XdslOptimizationLevel.High && FieldAccessor.CanOptimize(fieldInfo)) {
				_accessor = FieldAccessor.Create(fieldInfo);
			}
		}
		else if (memberInfo is PropertyInfo propertyInfo) {
			Type = propertyInfo.PropertyType;

			if (options.OptimizationLevel >= XdslOptimizationLevel.Low && PropertyAccessor.CanOptimize(propertyInfo)) {
				_accessor = PropertyAccessor.Create(propertyInfo);
			}
		}
		else {
			throw new InvalidOperationException();
		}

		UnderlyingMember = memberInfo;
		PropertyType = TypeUtility.GetPropertyType(Type);

		if (memberInfo.TryGetAttribute<XdslFormerlySerializedAsAttribute>(out var formallyAs)) {
			FormerlySerializedAs = formallyAs.Name;
		}

		SerializeByRef = memberInfo.HasAttribute<XdslSerializeReferenceAttribute>();
		Encoding = Type == typeof(string)
			? memberInfo.GetAttributeValue<XdslEncodedAttribute, string?>(x => x.Encoding, null)
			: null;

		IsRequired = memberInfo.HasAttribute<XdslRequiredAttribute>();

		_isEscaped = Type == typeof(string) && memberInfo.HasAttribute<XdslEscapedAttribute>();

		if (memberInfo.TryGetAttribute<XdslNameAttribute>(out var xdslName)) {
			Name = xdslName.Name;
		}
		else {
			Name = memberInfo.Name;
		}
	}

	#region Get & Set

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public object? GetValue(object target)
	{
		object? value;

		if (_accessor is not null) {
			value = _accessor.GetValue(target);
		}
		else if (UnderlyingMember is FieldInfo fieldInfo) {
			value = fieldInfo.GetValue(target);
		}
		else {
			value = ((PropertyInfo)UnderlyingMember).GetValue(target);
		}

		if (!string.IsNullOrEmpty(Encoding)) {
			return XdslEncoding.GetEncoder(Encoding).Encode((string?)value);
		}

		if (_isEscaped && value != null) {
			return ((string)value).Escape();
		}

		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetValue(object target, object? value, bool isValueEncoded)
	{
		if (!string.IsNullOrEmpty(Encoding) && /*Is the incoming value encoded.*/ isValueEncoded) {
			value = XdslEncoding.GetEncoder(Encoding).Decode((string?)value);
		}
		else if (_isEscaped && value != null) {
			value = ((string)value).UnEscape();
		}

		if (_accessor is not null) {
			_accessor.SetValue(target, value);
		}
		else if (UnderlyingMember is FieldInfo fieldInfo) {
			fieldInfo.SetValue(target, value);
		}
		else if (UnderlyingMember is PropertyInfo propertyInfo) {
			propertyInfo.SetValue(target, value);
		}
	}

	#endregion Get & Set

	#region Quick Access

	public bool TryGetAttribute(Type type, [NotNullWhen(true)] out Attribute? attribute)
	{
		attribute = UnderlyingMember.GetCustomAttribute(type);

		return attribute != null;
	}

	public bool TryGetAttribute<TAttribute>([NotNullWhen(true)] out TAttribute? attribute) where TAttribute : Attribute
	{
		attribute = UnderlyingMember.GetCustomAttribute<TAttribute>();

		return attribute != null;
	}

	public bool HasAttribute<TAttribute>() where TAttribute : Attribute
	{
		return Attribute.IsDefined(UnderlyingMember, typeof(TAttribute));
	}

	#endregion Quick Access
}