using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Serialization;

public sealed class XdslConverterCollection : List<IXdslConverter>
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Type converterType)
	{
		for (int i = 0; i < Count; i++) {
			if (this[i].GetType() == converterType)
				return true;
		}

		return false;
	}
}