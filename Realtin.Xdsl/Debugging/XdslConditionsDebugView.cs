using System;
using System.Diagnostics;

namespace Realtin.Xdsl.Debugging;

internal sealed class XdslConditionsDebugView(XdslConditions conditions)
{
	private readonly XdslConditions _conditions
		= conditions ?? throw new ArgumentNullException();

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public XdslCondition[] Items
	{
		get {
			XdslCondition[] array = new XdslCondition[_conditions.Count];

			_conditions._conditions.CopyTo(array, 0);

			return array;
		}
	}
}