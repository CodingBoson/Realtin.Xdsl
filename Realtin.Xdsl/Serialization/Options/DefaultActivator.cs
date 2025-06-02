using System;
using System.Reflection;
using System.Runtime.Serialization;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Serialization;

public sealed class DefaultActivator : XdslActivator
{
	private static readonly Type[] _emptyTypes = [];

	private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private readonly Type _type;

	private readonly bool _hasDefaultCtor;

	private DefaultActivator(Type type)
	{
		_type = type ?? throw new ArgumentNullException();

		if (type.IsStruct()) {
			_hasDefaultCtor = true;
		}
		else {
			_hasDefaultCtor = type.GetConstructor(_flags, null, _emptyTypes, null) != null;
		}
	}

    public override bool CanCreateInstance(Type type) => _type == type;

    public object CreateInstance()
	{
		if (_hasDefaultCtor) {
			return Activator.CreateInstance(_type, true);
		}

		try {
			return FormatterServices.GetSafeUninitializedObject(_type);
		}
		catch (Exception ex) {
			throw new XdslSerializerException($"Failed to get an uninitialized object of type {_type}", ex);
		}
	}

	public object? CreateInstanceDefaultCtor()
	{
		if (_hasDefaultCtor) {
			return Activator.CreateInstance(_type, true);
		}

		return null;
	}

	public override object CreateInstance(Type type, XdslSerializerOptions options)
	{
		if (_hasDefaultCtor) {
			return Activator.CreateInstance(_type, true);
		}

		try {
			return FormatterServices.GetSafeUninitializedObject(_type);
		}
		catch (Exception ex) {
			throw new XdslSerializerException($"Failed to get an uninitialized object of type {_type}", ex);
		}
	}

    public static DefaultActivator Create(Type type) => new DefaultActivator(type);
}