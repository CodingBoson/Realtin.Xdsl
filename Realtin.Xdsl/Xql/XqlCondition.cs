using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Realtin.Xdsl.Xql;

/// <summary>
/// Provides a class that represents a compiled XQL condition.
/// </summary>
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public sealed class XqlCondition : IXqlCondition, IEquatable<XqlCondition?>
{
	internal sealed class CompiledPropertyExpressions
	{
		private List<CompiledPropertyExpression>? _list;

		private CompiledPropertyExpression _item0 = default!;

		private int _count;

		public int Count => _count;

		public CompiledPropertyExpression this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if (index == 0) {
					return _item0;
				}

				return _list![index];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(CompiledPropertyExpression value)
		{
			if (_count == 1) {
				_list = [_item0, value];
			}
			else if (_count > 1) {
				_list!.Add(value);
			}
			else {
				_item0 = value;
			}

			_count++;
		}
	}

	internal sealed class CompiledPropertyExpression(XqlProperty property, string? argument = null)
	{
		public readonly XqlProperty Property = property;

		public readonly string? Argument = argument;

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Argument)) {
				return $"{Property}";
			}

			return $"{Property.ToString().ToUpperInvariant()}[{Argument}]";
		}
	}

	private readonly CompiledPropertyExpressions _propertyExpressions;

	/// <summary>
	/// The operator to use in the compression process.
	/// </summary>
	public readonly XqlOperator Operator;

	/// <summary>
	/// This condition's value.
	/// </summary>
	public readonly string Value;

	internal XqlCondition(CompiledPropertyExpressions expressions, XqlOperator @operator, string value)
	{
		_propertyExpressions = expressions;
		Value = value;
		Operator = @operator;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsConditionMet(XdslElement element, XqlVariables variables)
	{
		var met = IsConditionMetImpl(element, variables);

		return Operator == XqlOperator.Equals ? met : !met;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsConditionMetImpl(XdslElement element, XqlVariables variables)
	{
		var elem = element;
		var value = Value;

		if (value.StartsWith("${") && value.EndsWith("}")) {
			//foreach (var key in variables.Keys) {
			//	if (key.AsSpan().Equals(value.AsSpan(2, value.Length - 3), StringComparison.Ordinal)) {
			//		value = variables[key];
			//	}
			//}

			// TODO: Find a solution to the substring operation.
			if (!variables.TryGetValue(value[2..^1], out var variable)) {
				throw new XqlException($"Variable '{value}' was not found.");
			}

			value = variable;
		}

		var length = _propertyExpressions.Count;
		for (int i = 0; i < length; i++) {
			var expression = _propertyExpressions[i];
			var property = expression.Property;

			if (property == XqlProperty.Name) {
				return elem.Name == value;
			}
			else if (property == XqlProperty.Type) {
				return elem.NodeType == XqlParser.ParseNodeType(value);
			}
			else if (property == XqlProperty.Text) {
				return elem.Text == value;
			}
			else if (property == XqlProperty.Child) {
				if (string.IsNullOrEmpty(expression.Argument)) {
					return elem.GetChild(value) != null;
				}

				var child = elem.GetChild(expression.Argument)
					?? throw new XqlException($"Child '{expression.Argument}' was not found in '{elem.Name}'.");

				elem = child;
			}
			else if (property == XqlProperty.Attribute) {
				if (string.IsNullOrEmpty(expression.Argument)) {
					return elem.HasAttributes && elem.GetAttribute(value) != null;
				}

				var attribute = elem.GetAttribute(expression.Argument)
					?? throw new XqlException($"Attribute '{expression.Argument}' was not found in '{elem.Name}'.");

				return _propertyExpressions[i + 1].Property switch {
					XqlProperty.Text => attribute.Value == value,
					_ => throw new XqlException(),
				};
			}
		}

		throw new XqlException();
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj) => Equals(obj as XqlCondition);

	/// <inheritdoc/>
	public bool Equals(XqlCondition? other)
	{
		return other is not null &&
			   EqualityComparer<CompiledPropertyExpressions>.Default.Equals(_propertyExpressions, other._propertyExpressions) &&
			   Operator == other.Operator &&
			   Value == other.Value;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(_propertyExpressions, Operator, Value);
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		return $"X:{string.Join(":", _propertyExpressions)} {(Operator == XqlOperator.Equals ? "==" : "!=")} \"{Value}\"";
	}

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XqlCondition"/> objects are equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
	public static bool operator ==(XqlCondition? left, XqlCondition? right)
	{
		if (ReferenceEquals(left, right))
			return true;

		if (left is null)
			return false;

		return left.Equals(right);
	}

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XqlCondition"/> objects are not equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
	public static bool operator !=(XqlCondition? left, XqlCondition? right)
	{
		return !(left == right);
	}
}