using System;

namespace Realtin.Xdsl.Linq;

/// <summary>
/// Represents a find query in XDSL LINQ.
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <param name="query"></param>
/// <param name="deep"></param>
public readonly struct FindQuery<TResult>(QueryAction<TResult> query, bool deep) : IQuery<TResult> where TResult : XdslElement
{
	private readonly QueryAction<TResult> _query = query
		?? throw new ArgumentNullException(nameof(query));

	private readonly bool _deep = deep;

	/// <inheritdoc/>
	public TResult? Query(XdslNode node)
	{
		var children = node.Children;

		if (children is null) {
			return default;
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];
			var t = child as TResult;

			if (t != null && _query(t)) {
				return t;
			}

			if (_deep && child is not XdslTag && child is not XdslComment
				&& child.Children is not null) {
				var t2 = Query(child);

				if (t2 != null) {
					return t2;
				}
			}
		}

		return default;
	}
}