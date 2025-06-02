using System;
using System.Collections.Generic;

namespace Realtin.Xdsl.Linq;

/// <summary>
/// Represents a where query in XDSL LINQ.
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <param name="query"></param>
/// <param name="deep"></param>
public readonly struct WhereQuery<TResult>(QueryAction<TResult> query, bool deep)
	: IQuery<IEnumerable<TResult>> where TResult : XdslElement
{
	private readonly QueryAction<TResult> _query = query ?? throw new ArgumentNullException(nameof(query));

	private readonly bool _deep = deep;

	/// <inheritdoc/>
	public IEnumerable<TResult> Query(XdslNode node)
	{
		var children = node.Children;

		if (children is null) {
			yield break;
		}

		for (int i = 0; i < children.Count; i++) {
			var child = children[i];
			var t = child as TResult;

			if (t != null && _query(t)) {
				yield return t;
			}

			if (_deep && child is not XdslTag && child is not XdslComment
				&& child.Children is not null) {
				foreach (var item in Query(child)) {
					yield return item;
				}
			}
		}
	}
}