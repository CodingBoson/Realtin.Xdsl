using System;
using System.Reflection;

namespace Realtin.Xdsl.Utilities;

internal static class ReflectionUtility
{
	public static bool IsPublic(this PropertyInfo propertyInfo)
	{
		if (propertyInfo.CanRead)
			return propertyInfo.GetGetMethod(true).IsPublic;

		if (propertyInfo.CanWrite)
			return propertyInfo.GetSetMethod(true).IsPublic;

		//N/A
		throw new NotSupportedException();
	}
}