using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;
using Realtin.Xdsl.Xql.Compilers;

namespace Realtin.Xdsl.Xql;

/// <summary>
/// Provides a class that represents a compiled XQL expression.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class XqlExpression : IEquatable<XqlExpression?>
{
	/// <summary>
	/// Gets the query for this expression.
	/// </summary>
	public XqlQueryType Query { get; }

	/// <summary>
	/// Gets the XQL operation for this expression.
	/// </summary>
	public XqlOperation Operation { get; }

	/// <summary>
	/// Gets a list of all conditions in this expression.
	/// </summary>
	public IList<XqlCondition> Conditions { get; }

	/// <summary>
	/// Initialize a new instance of the <see cref="XqlExpression"/> class.
	/// </summary>
	/// <param name="query"></param>
	/// <param name="method"></param>
	/// <param name="conditions"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public XqlExpression(XqlQueryType query, XqlOperation method, IList<XqlCondition> conditions)
	{
		Query = query;
		Operation = method;
		Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
	}

	/// <summary>
	/// Executes this expression on the specified node.
	/// </summary>
	/// <param name="node">The node to execute this expression on.</param>
	/// <param name="permissions">Access to the node's content.</param>
	/// <returns></returns>
	/// <param name="variables"></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XqlResult Execute(XdslNode node, XqlPermissions permissions, XqlVariables? variables = null)
	{
		return Execute(node, true, permissions, variables);
	}

	/// <summary>
	/// Executes this expression on the specified node.
	/// </summary>
	/// <param name="node">The node to execute this expression on.</param>
	/// <param name="deep">Include the node's children in the query.</param>
	/// <param name="permissions">Access to the node's content.</param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	/// <param name="variables"></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XqlResult Execute(XdslNode node, bool deep = true, XqlPermissions permissions = XqlPermissions.Read, XqlVariables? variables = null)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(node), node);
		variables ??= XqlVariables.Empty;

		if (!HasValidPermissions(node, permissions, out var message)) {
			return XqlResult.Failed(message);
		}

		if (Operation == XqlOperation.Select) {
			if (Query == XqlQueryType.Where) {
				var elements = WhereSelectHelper(node, deep, variables);

				return XqlResult.Succeed(elements);
			}
			else if (Query == XqlQueryType.First) {
				var element = FirstSelectHelper(node, deep, variables);

				return XqlResult.Succeed(element);
			}

			throw new NotImplementedException();
		}
		else if (Operation == XqlOperation.Delete) {
			if (Query == XqlQueryType.Where) {
				WhereDeleteHelper(node, deep, variables);

				return XqlResult.Succeed(null);
			}
			else if (Query == XqlQueryType.First) {
				FirstDeleteHelper(node, deep, variables);

				return XqlResult.Succeed(null);
			}
		}

		throw new NotImplementedException();
	}

	#region Validation

	private bool HasValidPermissions(XdslNode node, XqlPermissions permissions, out string message)
	{
		if (Operation == XqlOperation.Select && (permissions & XqlPermissions.Read) == 0) {
			message = $"Access to {node.Name} is denied. User does not have read permissions.";

			return false;
		}

		if (Operation == XqlOperation.Delete && (permissions & XqlPermissions.Write) == 0) {
			message = $"Access to {node.Name} is denied. User does not have write permissions.";

			return false;
		}

		message = "";

		return true;
	}

	#endregion Validation

	#region Equality

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		return Equals(obj as XqlExpression);
	}

	/// <inheritdoc/>
	public bool Equals(XqlExpression? other)
	{
		return other is not null &&
			   Query == other.Query &&
			   Operation == other.Operation &&
			   EqualityComparer<IList<XqlCondition>>.Default.Equals(Conditions, other.Conditions);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		return HashCode.Combine(Query, Operation, Conditions);
	}

	#endregion Equality

	#region Query Helpers

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private List<XdslElement> WhereSelectHelper(XdslNode node, bool deep, XqlVariables variables)
	{
		var result = new List<XdslElement>();

		var children = node.Children;

		WhereSelectHelper(deep, result, children, variables);

		return result;
	}

	// Use a for loop instead of Linq to save memory.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WhereSelectHelper(bool deep, List<XdslElement> result, XdslElementCollection? children, XqlVariables variables)
	{
		if (children is null) {
			return;
		}

		var length = children.Count;
		for (int i = 0; i < length; i++) {
			var child = children[i];

			if (Match(child, variables)) {
				result.Add(child);
			}

			if (deep && child is not XdslTag && child is not XdslComment) {
				WhereSelectHelper(deep, result, child.Children, variables);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private XdslElement? FirstSelectHelper(XdslNode node, bool deep, XqlVariables variables)
	{
		//return new FindQuery<XdslElement>(Match, deep).Query(node);

		//return new FindQuery<XdslElement>((elem) => Match(elem, variables), deep).Query(node);

		var children = node.Children;

		if (children is null) {
			return null;
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];

			if (Match(child, variables)) {
				return child;
			}

			if (deep && child is not XdslTag && child is not XdslComment
				&& child.Children is not null) {
				var result = FirstSelectHelper(child, true, variables);

				if (result != null) {
					return result;
				}
			}
		}

		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void WhereDeleteHelper(XdslNode node, bool deep, XqlVariables variables)
	{
		var children = node.Children;

		if (children is null) {
			return;
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];

			if (Match(child, variables)) {
				node.RemoveChild(child);
				i--;

				continue;
			}

			if (deep && child is not XdslTag && child is not XdslComment
				&& child.Children is not null) {
				WhereDeleteHelper(child, deep, variables);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool FirstDeleteHelper(XdslNode node, bool deep, XqlVariables variables)
	{
		var children = node.Children;

		if (children is null) {
			return false;
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];

			if (Match(child, variables)) {
				node.RemoveChild(child);

				return true;
			}

			if (deep && child is not XdslTag && child is not XdslComment
				&& child.Children is not null) {
				if (FirstDeleteHelper(child, deep, variables)) {
					return true;
				}
			}
		}

		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool Match(XdslElement element, XqlVariables variables)
	{
		var length = Conditions.Count;

		for (int i = 0; i < length; i++) {
			var condition = Conditions[i];

			if (!condition.IsConditionMet(element, variables)) {
				return false;
			}
		}

		return true;
	}

	#endregion Query Helpers

	#region Compiling

	/// <summary>
	/// Compiles an <see cref="XqlExpression"/> object from an XQL expression <see cref="string"/>.
	/// </summary>
	/// <param name="expression">A <see cref="string"/> representing an XQL expression.</param>
	/// <returns></returns>
	/// <exception cref="XqlException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlExpression Compile(string expression)
	{
		return XqlExpressionCompiler.Compile(expression);
	}

	/// <summary>
	/// Compiles an <see cref="XqlExpression"/> object from an XQL expression <see cref="string"/>.
	/// </summary>
	/// <param name="expression">A <see cref="ReadOnlySpan{T}"/> of characters representing an XQL expression.</param>
	/// <returns></returns>
	/// <exception cref="XqlException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XqlExpression Compile(ReadOnlySpan<char> expression)
	{
		return XqlExpressionCompiler.Compile(expression);
	}

	#endregion Compiling

	#region Operators

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XqlExpression"/> objects are equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
	public static bool operator ==(XqlExpression? left, XqlExpression? right)
	{
		if (ReferenceEquals(left, right)) {
			return true;
		}

		if (left is null) {
			return false;
		}

		return left.Equals(right);
	}

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XqlExpression"/> objects are not equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
	public static bool operator !=(XqlExpression? left, XqlExpression? right)
	{
		return !(left == right);
	}

	#endregion Operators

	#region Debugging

	private string GetDebuggerDisplay()
	{
		return $"{Query.ToString().ToUpperInvariant()} {string.Join(" & ", Conditions)} {Operation.ToString().ToUpperInvariant()}";
	}

	#endregion Debugging
}