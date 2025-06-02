using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Parsers;
using Realtin.Xdsl.Pooling;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl;

// Hierarchical XDSL Parser.
internal sealed class XdslTextReader
{
	private readonly string _chars;

	private readonly int _length;

	private readonly Stack<XdslNode> _hierarchy;

	private int _charPosition;

	private XdslNode _current;

	private int Depth => _hierarchy.Count;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public XdslTextReader(string xdsl)
	{
		_chars = xdsl ?? throw new ArgumentNullException(nameof(xdsl));
		//Skip any white space at the end.
		_length = ((ReadOnlySpan<char>)xdsl).TrimEnd().Length;
		_charPosition = 0;
		_current = default!;

		_hierarchy = StackPool<XdslNode>.Rent();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Load(XdslDocument document, XdslDocumentOptions options)
	{
		_current = document;

		try {
			LoadSequence(options);
		}
		catch (Exception ex) when (ex is not XdslException) {
			throw new XdslException("Unexpected error while loading an XdslDocument.", ex);
		}
		finally {
			StackPool<XdslNode>.Return(_hierarchy);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LoadSequence(XdslDocumentOptions options)
	{
		while (_charPosition < _length) {
			var node = ReadNextNode(options);

			if (!LoadNode(node, options)) {
				LoadNodeContent();
			}
		}

		if (Depth == 1) {
			throw new XdslException($"XNode {_current.XNode} does not have a closing XNode '</{_current.Name}>'.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ReadOnlySpan<char> ReadNextNode(XdslDocumentOptions options)
	{
		var chars = (ReadOnlySpan<char>)_chars;
		var escapingDepth = 0;

		for (int i = _charPosition; i < _length; i++) {
			char ci = chars[i];
			_charPosition++;

			if (ci == '@' && i < _length - 1 && chars[i + 1] == '{') {
				escapingDepth++;

				continue;
			}

			if (escapingDepth > 0) {
				if (ci == '}')
					escapingDepth--;

				continue;
			}

			if (ci == '<') {
				int num = 0;

				if (options.CommentHandling == XdslCommentHandling.Parse) {
					bool isComment = i < _length - 3 && chars[i + 1] == '!' && chars[i + 2] == '-' && chars[i + 3] == '-';

					if (isComment) {
						// Handle comments.
						for (int j = i + 1; j < _length; j++) {
							char cj = chars[j];
							_charPosition++;

							if (cj == '-' && chars[j + 1] == '-' && chars[j + 2] == '>') {
								num += 4;

								return chars.Slice(i, num);
							}
							else if (cj == '<' && i < _length - 3 && chars[j + 1] == '!' && chars[j + 2] == '-' && chars[j + 3] == '-') {
								throw new XdslException($"Comment cannot contain a nested comment.");
							}

							num++;
						}
					}
					else {
						for (int j = i + 1; j < _length; j++) {
							char charAtJ = chars[j];

							_charPosition++;

							if (charAtJ == '>') {
								num += 2;

								return chars.Slice(i, num);
							}
							else if (charAtJ == '<') {
								throw new XdslException($"Name cannot contain the '<' character.");
							}

							num++;
						}
					}
				}
				else {
					for (int j = i + 1; j < _length; j++) {
						char charAtJ = chars[j];
						_charPosition++;

						if (charAtJ == '>') {
							num += 2;

							return chars.Slice(i, num);
						}
						else if (charAtJ == '<') {
							throw new XdslException($"Name cannot contain the '<' character.");
						}

						num++;
					}
				}

				throw new XdslException($"XNode does not end with the '>' character.");
			}
		}

		throw new XdslException("Invalid token 'Text' at root level of document.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LoadNodeContent()
	{
		ReadOnlySpan<char> chars = _chars;
		int num = 0;
		int escapingDepth = 0;

		for (int i = _charPosition; i < _length; i++) {
			char c = chars[i];

			if (c == '@' && i < _length - 1 && chars[i + 1] == '{') {
				escapingDepth++;

				continue;
			}

			if (escapingDepth > 0) {
				if (c == '}') {
					escapingDepth--;
				}

				num++;
				continue;
			}

			if (c == '<') {
				var text = chars.Slice(_charPosition, num).Trim();

				_current.Text = text.ToString();

				break;
			}

			num++;
		}

		_charPosition += num;
	}

	// true if the XNode is a self enclosing type.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool LoadNode(ReadOnlySpan<char> node, XdslDocumentOptions options)
	{
		if (node.StartsWith("</", StringComparison.OrdinalIgnoreCase)) {
			var curName = (ReadOnlySpan<char>)_current.Name;

			if (!curName.Equals(node[2..^1], StringComparison.Ordinal)) {
				throw new XdslException($"XNode {_current.XNode} does not have a closing XNode '</{_current.Name}>'.");
			}

			_current = _hierarchy.Pop();

			return true;
		}

		switch (node[1]) {
			case '!':
				if (IsXdslComment(node)) {
					if (options.CommentHandling == XdslCommentHandling.Ignore) {
						return true;
					}

					_current.AppendChild(new XdslComment(node[4..^3].ToString()));
				}
				else {
					throw new XdslException($"Invalid XNode {node.ToString()}.");
				}

				return true;

			case '?':
				if (IsXdslTag(node)) {
					if (options.TagHandling == XdslTagHandling.Ignore) {
						return true;
					}

					DeconstructXNode(node[2..^2], out var tagName, out var tagAttributes);

					var tag = new XdslTag(tagName.ToString());

					if (tagAttributes.Length > 0) {
						LoadAttributes(tag, tagAttributes);
					}

					_current.AppendChild(tag);
				}
				else {
					throw new XdslException($"Invalid XNode {node.ToString()}.");
				}

				return true;

			default:
				if (node.EndsWith("/>", StringComparison.OrdinalIgnoreCase)) {
					DeconstructXNode(node[1..^2], out var eName, out var eAttributes);

					var element = new XdslElement(eName.ToString());

					if (eAttributes.Length > 0) {
						LoadAttributes(element, eAttributes);
					}

					_current.AppendChild(element);

					return true;
				}

				DeconstructXNode(node[1..^1], out var name, out var attributes);

				var current = new XdslElement(name.ToString());

				if (attributes.Length > 0) {
					LoadAttributes(current, attributes);
				}

				_current.AppendChild(current);

				_hierarchy.Push(_current);

				_current = current;

				return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeconstructXNode(ReadOnlySpan<char> xNode, out ReadOnlySpan<char> name, out ReadOnlySpan<char> attributes)
	{
		int num = xNode.IndexOf(' ');

		if (num > 0) {
			name = xNode[..num];
			attributes = xNode[num..].TrimStart();
		}
		else {
			name = xNode;
			attributes = default;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void LoadAttributes(XdslNode node, ReadOnlySpan<char> attributes)
	{
		var reader = new XdslAttributeReader(attributes);

		while (reader.Read()) {
			var (attributeName, attributeValue) = reader.Current;

			node.AddAttribute(new XdslAttribute(attributeName, attributeValue));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsXdslComment(ReadOnlySpan<char> xNode)
	{
		return xNode.StartsWith("<!--", StringComparison.OrdinalIgnoreCase)
			&& xNode.EndsWith("-->", StringComparison.OrdinalIgnoreCase);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsXdslTag(ReadOnlySpan<char> xNode)
	{
		return xNode.StartsWith("<?", StringComparison.OrdinalIgnoreCase)
			&& xNode.EndsWith("?>", StringComparison.OrdinalIgnoreCase);
	}
}