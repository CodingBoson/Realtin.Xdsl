using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

/// <summary>
/// Base class for all nodes in XDSL.
/// </summary>
public abstract class XdslNode : IEquatable<XdslNode?>, IEnumerable<XdslNode>
{
	/// <summary>
	/// Supports a simple iteration over an <see cref="XdslNode"/>.
	/// </summary>
	/// <param name="node"></param>
	public struct Enumerator(XdslNode node) : IEnumerator<XdslNode>
	{
		private readonly XdslNode _node = node;

		private int _index;

		private XdslNode _current = default!;

		/// <inheritdoc/>
		public readonly XdslNode Current => _node;

		/// <inheritdoc/>
		readonly object IEnumerator.Current => _current;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			var children = _node.Children;

			if (children is null) {
				return false;
			}

			if (_index >= children.Count) {
				_current = default!;

				return false;
			}

			_current = children[_index++];

			return true;
		}

		/// <inheritdoc/>
		public void Reset()
		{
			_index = 0;
			_current = default!;
		}

		readonly void IDisposable.Dispose()
		{
		}
	}

	/// <summary>
	/// Gets or Sets the name of this node.
	/// </summary>
	public virtual string Name { get; set; }

    /// <summary>
    /// Returns a value that indicates whether this node's name is special.
    /// <para>
    /// <see langword="xdsl:import"/>
    /// </para>
    /// </summary>
    public virtual bool IsSpecialName => Name.Contains(":", StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Gets the opening node of this node.
	/// <para>Includes the name and attributes, '<![CDATA[<MyNode name="Node">]]>'</para>
	/// </summary>
	public virtual string XNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (Attributes is null) {
				return string.Create(Name.Length + 2, Name, (span, state) => {
					span[0] = '<';

					state.AsSpan().CopyTo(span[1..^1]);

					span[^1] = '>';
				});
			}

			return $"<{Name} {Attributes}>";
		}
	}

	/// <summary>
	/// Gets or Sets the text of this node.
	/// </summary>
	public virtual string? Text { get; set; }

	/// <summary>
	/// Gets the type of node this is.
	/// </summary>
	public abstract XdslNodeType NodeType { get; }

	/// <summary>
	/// The attributes of this node.
	/// </summary>
	public virtual XdslAttributeCollection? Attributes { get; protected internal set; }

	/// <summary>
	/// Gets a list of all child elements in this node.
	/// </summary>
	public virtual XdslElementCollection? Children { get; protected internal set; }

	/// <summary>
	/// Returns a value that indicates whether this node contains any child elements.
	/// </summary>
	public virtual bool HasChildNodes => Children is not null;

	/// <summary>
	/// Returns a value that indicates whether this node contains any attributes.
	/// </summary>
	public virtual bool HasAttributes => Attributes is not null;

	/// <summary>
	/// The total count of all child nodes including their children.
	/// </summary>
	public virtual int TotalCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get {
			if (Children is null) {
				return 0;
			}

			int sum = Children.Count;

			for (int i = 0; i < Children.Count; i++)
				sum += Children[i].TotalCount;

			return sum;
		}
	}

	/// <summary>
	/// Is this node empty?
	/// </summary>
	public virtual bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Attributes is null
			&& Children is null
			&& string.IsNullOrEmpty(Text);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="XdslNode"/> class.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="text"></param>
	/// <exception cref="ArgumentNullException"></exception>
	protected XdslNode(string name, string? text)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Text = text;
	}

	#region Attributes API

	/// <summary>
	/// Adds the specified <paramref name="attribute"/>.
	/// <para>Note: Attributes must have unique names.</para>
	/// </summary>
	/// <param name="attribute"></param>
	/// <exception cref="XdslException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAttribute(XdslAttribute attribute)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(attribute), attribute);

		if (HasAttribute(attribute.Name)) {
			throw new XdslException($"Duplicated attribute '{attribute.Name}'");
		}

		Attributes ??= [];

		Attributes.Add(attribute);
	}

	/// <summary>
	/// Adds a new attribute with the specified <paramref name="name"/> and <paramref name="value"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <exception cref="XdslException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAttribute(string name, string value) => AddAttribute(new XdslAttribute(name, value));

	/// <summary>
	/// Moves an attribute from it's current position to a new position specified
	/// by the <paramref name="newPosition"/> parameter.
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="attribute"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException">The specified attribute is not in this node.</exception>
	public void MoveAttribute(int newPosition, XdslAttribute attribute)
	{
		if (attribute is null) {
			throw new ArgumentNullException(nameof(attribute));
		}

		if (Attributes is null) {
			throw new ArgumentException("The specified attribute is not in this node.", nameof(Attributes));
		}

		int oldPosition = Attributes.IndexOf(attribute);

		if (oldPosition < 0) {
			throw new ArgumentException("The specified attribute is not in this node.", nameof(Attributes));
		}

		Attributes.RemoveAt(oldPosition);
		Attributes.Insert(newPosition, attribute);
	}

	/// <summary>
	/// Moves an attribute from it's current position to a new position specified
	/// by the <paramref name="newPosition"/> parameter.
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="attributeName">The name of the attribute to move.</param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException">The specified attribute is not in this node.</exception>
	public void MoveAttribute(int newPosition, string attributeName)
	{
		MoveAttribute(newPosition, GetAttribute(attributeName)!);
	}

	/// <summary>
	/// Removes the specified <paramref name="attribute"/>.
	/// </summary>
	/// <param name="attribute"></param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="attribute"/> is successfully removed;
	/// otherwise, <see langword="false"/>. This method also returns
	/// <see langword="false"/> if <paramref name="attribute"/> was not found.
	/// </returns>
	public bool RemoveAttribute(XdslAttribute attribute)
	{
		if (Attributes is null) {
			return false;
		}

		return Attributes.Remove(attribute);
	}

	/// <summary>
	/// Removes an attribute with the specified <paramref name="attributeName"/>.
	/// </summary>
	/// <param name="attributeName"></param>
	/// <returns>
	/// <see langword="true"/> if attribute is successfully removed;
	/// otherwise, <see langword="false"/>. This method also returns
	/// <see langword="false"/> if attribute was not found.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveAttribute(string attributeName) => RemoveAttribute(GetAttribute(attributeName)!);

	/// <summary>
	/// Returns a value that indicates whether this node has
	/// an attribute with the specified <paramref name="attributeName"/>.
	/// </summary>
	/// <param name="attributeName"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasAttribute(string attributeName)
	{
		if (Attributes is null) {
			return false;
		}

		for (int i = 0; i < Attributes.Count; i++) {
			var attribute = Attributes[i];

			if (attribute.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)) {
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the attribute with the same name as the specified one.
	/// </summary>
	/// <param name="attributeName"></param>
	/// <returns>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslAttribute? GetAttribute(string attributeName)
	{
		if (Attributes is null) {
			return null;
		}

		int count = Attributes.Count;
		for (int i = 0; i < count; i++) {
			var attribute = Attributes[i];

			if (attribute.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase)) {
				return attribute;
			}
		}

		return null;
	}

	/// <summary>
	/// Gets the attribute with the same name as the specified one.
	/// </summary>
	/// <param name="attributeName"></param>
	/// <returns>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslAttribute GetOrAddAttribute(string attributeName)
	{
		var attribute = GetAttribute(attributeName);

		if (attribute is null) {
			attribute = new XdslAttribute(attributeName, string.Empty);

			AddAttribute(attribute);
		}

		return attribute;
	}

	/// <summary>
	/// Tries to get an attribute with the same name as the specified one.
	/// </summary>
	/// <param name="attributeName"></param>
	/// <param name="attribute"></param>
	/// <returns>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetAttribute(string attributeName, [NotNullWhen(true)] out XdslAttribute? attribute)
	{
		attribute = GetAttribute(attributeName);

		return attribute != null;
	}

	#endregion Attributes API

	#region Child API

	/// <summary>
	/// Creates a new <see cref="XdslElement"/> and adds it to the end of this node.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslElement CreateElement(string name, Action<XdslElement>? action = default)
	{
		var element = new XdslElement(name);

		AppendChild(element);

		action?.Invoke(element);

		return element;
	}

	/// <summary>
	/// Creates a new <see cref="XdslTag"/> and adds it to the end of this node.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslTag CreateTag(string name, Action<XdslTag>? action = default)
	{
		var tag = new XdslTag(name);

		AppendChild(tag);

		action?.Invoke(tag);

		return tag;
	}

	/// <summary>
	/// Creates a new <see cref="XdslComment"/> using the specified <paramref name="comment"/>
	/// and adds it to the end of this node.
	/// </summary>
	/// <param name="comment"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslComment CreateComment(string comment)
	{
		var commentElement = new XdslComment(comment);

		AppendChild(commentElement);

		return commentElement;
	}

	/// <summary>
	/// Adds the specified element to the end of this node.
	/// </summary>
	/// <param name="child"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public void AppendChild(XdslElement child)
	{
		if (child is null) {
			throw new ArgumentNullException(nameof(child));
		}

		Children ??= [];

		child.Parent = this;
		Children.Add(child);
	}

	/// <summary>
	/// Removes a child element from this node.
	/// </summary>
	/// <param name="child"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException">This is not my child.</exception>
	public void RemoveChild(XdslElement child)
	{
		if (child is null) {
			throw new ArgumentNullException(nameof(child));
		}

		if (child.Parent != (object)this) {
			throw new ArgumentException("This is not my child.", nameof(child));
		}

		child.Parent = null!;
		Children!.Remove(child);
	}

	/// <summary>
	/// Gets the first child with the same name as the specified one.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslElement? GetChild(string name)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(name), name);

		if (Children is null) {
			return null;
		}

		for (int i = 0; i < Children.Count; i++) {
			var child = Children[i];

			if (child.Name == name) {
				return child;
			}
		}

		return null;
	}

	/// <summary>
	/// Gets the first child that satisfies the specified condition.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	/// <param name="state"></param>
	/// <param name="filter">A function to test each element for a condition.</param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslElement? GetChild<TState>(TState state, Func<TState, XdslElement, bool> filter)
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(filter), filter);

		if (Children is null) {
			return null;
		}

		for (int i = 0; i < Children.Count; i++) {
			var child = Children[i];

			if (filter(state, child)) {
				return child;
			}
		}

		return null;
	}

	/// <summary>
	/// Returns a value that indicates whether this node has a child
	/// with the specified <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public bool HasChild(string name) => GetChild(name) is not null;

	/// <summary>
	/// Returns a value that indicates whether the specified <paramref name="element"/> is this node's child.
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public bool HasChild(XdslElement element)
	{
		return Children is not null && Children.Contains(element);
	}

	/// <summary>
	/// Returns a value that indicates whether this node contains a child that satisfies the specified condition.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	/// <param name="state"></param>
	/// <param name="filter">A function to test each element for a condition.</param>
	/// <returns></returns>
	public bool HasChild<TState>(TState state, Func<TState, XdslElement, bool> filter)
	{
		return GetChild(state, filter) is not null;
	}

	#endregion Child API

	#region Content Writing

	/// <summary>
	/// Writes this node's content to the specified <see cref="XdslTextWriter"/>.
	/// </summary>
	/// <param name="writer"></param>
	public abstract void WriteContentTo(XdslTextWriter writer);

	/// <summary>
	/// Writes this node's content to the specified <see cref="XdslTextWriter"/>.
	/// </summary>
	/// <param name="writer"></param>
	public abstract void WriteContentToIndented(XdslTextWriter writer);

	#endregion Content Writing

	#region Overrides & Interfaces

	/// <inheritdoc/>
	public override sealed bool Equals(object? obj) => Equals(obj as XdslNode);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool Equals(XdslNode? other)
	{
		return other is not null &&
			   Name == other.Name &&
			   NodeType == other.NodeType &&
			   StringUtility.Equals(Text, other.Text) &&
			   EqualityComparer<XdslAttributeCollection?>.Default.Equals(Attributes, other.Attributes) &&
			   EqualityComparer<XdslElementCollection?>.Default.Equals(Children, other.Children);
	}

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Name, Text, Attributes, Children);

	/// <inheritdoc/>
	public IEnumerator<XdslNode> GetEnumerator() => new Enumerator(this);

	IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

	#endregion Overrides & Interfaces

	#region Operators

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XdslNode"/> objects are equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool operator ==(XdslNode? left, XdslNode? right)
	{
		if (ReferenceEquals(left, right)) {
			return true;
		}

		return left is not null && left.Equals(right);
	}

	/// <summary>
	/// Returns a value that indicates whether two <see cref="XdslNode"/> objects are not equal.
	/// </summary>
	/// <param name="left">The first object to compare.</param>
	/// <param name="right">The second object to compare.</param>
	/// <returns>
	/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool operator !=(XdslNode? left, XdslNode? right) => !(left == right);

	#endregion Operators
}