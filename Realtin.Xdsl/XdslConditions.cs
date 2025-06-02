using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Realtin.Xdsl.Debugging;

namespace Realtin.Xdsl;

/// <summary>
/// Provides options for the <see cref="XdslConditionalElementProcessor"/> class.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(XdslConditionsDebugView))]
public sealed class XdslConditions : IXdslDocumentOptions<XdslConditions>, IEnumerable<XdslCondition>
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal readonly List<XdslCondition> _conditions;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public int Count => _conditions.Count;

	public XdslConditions() => _conditions = [];

	/// <summary>
	/// Appends a new condition to this list.
	/// </summary>
	/// <param name="condition"></param>
	/// <exception cref="ArgumentException"></exception>
	public void AddCondition(XdslCondition condition)
	{
		if (HasCondition(condition)) {
			throw new ArgumentException("XdslConditions cannot contain more than one condition with the same name.", nameof(condition));
		}

		_conditions.Add(condition);
	}

	public void AddCondition(string name, bool isChecked) => _conditions.Add(new XdslCondition(name, isChecked));

	public bool RemoveCondition(XdslCondition condition) => _conditions.Remove(condition);

	public XdslCondition? GetCondition(string name)
	{
		for (int i = 0; i < Count; i++) {
			var condition = _conditions[i];

			if (condition.Name == name) {
				return condition;
			}
		}

		return null;
	}

	public bool HasCondition(XdslCondition condition) => _conditions.Contains(condition);

	public bool HasCondition(string name) => GetCondition(name) is not null;

	/// <inheritdoc/>
	public XdslConditions Clone()
	{
		var clone = new XdslConditions();

		clone._conditions.Clear();
		clone._conditions.AddRange(_conditions);

		return clone;
	}

	/// <inheritdoc/>
	public XdslConditions Clone(Action<XdslConditions> with)
	{
		var clone = Clone();

		with(clone);

		return clone;
	}

	/// <inheritdoc/>
	public IEnumerator<XdslCondition> GetEnumerator() => _conditions.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}