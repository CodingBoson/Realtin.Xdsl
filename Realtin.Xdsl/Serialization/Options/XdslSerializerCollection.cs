using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

/// <summary>
/// Represents a collection of XdslSerializers. See also <see cref="XdslSerializer{T}"/>.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
public sealed class XdslSerializerCollection : List<XdslSerializer>
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslSerializer? GetSerializer(Type type)
	{
		int length = Count;
		for (int i = 0; i < length; i++) {
			var serializer = this[i];

			if (serializer.CanSerialize(type)) {
				return serializer;
			}
		}

		return null;
	}
}