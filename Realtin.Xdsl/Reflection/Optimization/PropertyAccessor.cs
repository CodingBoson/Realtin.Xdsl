using System;
using System.Reflection;
using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl.Optimization;

internal abstract class PropertyAccessor : IOptimizedAccessor
{
	public abstract void Compile(PropertyInfo propertyInfo);

	public abstract object? GetValue(object instance);

	public abstract void SetValue(object instance, object? value);

    public static bool CanOptimize(PropertyInfo property) => property.DeclaringType.IsClass;

    public static PropertyAccessor Create(PropertyInfo property)
	{
		try {
			var declaringType = property.DeclaringType;
			var propertyType = property.PropertyType;

			var accessorType = typeof(PropertyAccessor<,>).MakeGenericType(declaringType, propertyType);

			var accessor = (PropertyAccessor)Activator.CreateInstance(accessorType);

			accessor.Compile(property);

			return accessor;
		}
		catch (Exception ex) {
			throw new XdslSerializerException($"Failed to optimize property {property.Name} on {property.DeclaringType}.", ex);
		}
	}
}