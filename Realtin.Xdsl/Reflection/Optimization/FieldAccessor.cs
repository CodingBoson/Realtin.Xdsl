using System;
using System.Reflection;
using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl.Optimization;

internal abstract class FieldAccessor : IOptimizedAccessor
{
	public abstract void Compile(FieldInfo fieldInfo);

	public abstract object? GetValue(object target);

	public abstract void SetValue(object target, object? value);

    public static bool CanOptimize(FieldInfo field) => field.DeclaringType.IsClass;

    public static FieldAccessor Create(FieldInfo field)
	{
		try {
			var declaringType = field.DeclaringType;
			var fieldType = field.FieldType;

			var accessorType = typeof(FieldAccessor<,>).
				MakeGenericType(declaringType, fieldType);

			var accessor = (FieldAccessor)Activator.CreateInstance(accessorType);

			accessor.Compile(field);

			return accessor;
		}
		catch (Exception ex) {
			throw new XdslSerializerException($"Failed to optimize field {field.Name} on {field.DeclaringType}.", ex);
		}
	}
}