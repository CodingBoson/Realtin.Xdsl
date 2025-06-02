using System;
using System.Collections.Generic;
using Realtin.Xdsl.Utilities;

namespace Realtin.Xdsl.Linq;

/// <summary>
/// Provides extension methods for XDSL LINQ Queries.
/// </summary>
public static class LinqExtensions
{
	/// <summary>
	/// Perform a find query on the node.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="query"></param>
	/// <param name="deep"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static T? Find<T>(this XdslNode node, QueryAction<T> query, bool deep = false) where T : XdslElement
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(node), node);
		ThrowerHelper.ThrowIfArgumentNull(nameof(query), query);

		return new FindQuery<T>(query, deep).Query(node);
	}

	/// <summary>
	/// Perform a where query on the node.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="node"></param>
	/// <param name="query"></param>
	/// <param name="deep"></param>
	/// <returns></returns>
	public static IEnumerable<T> Where<T>(this XdslNode node, QueryAction<T> query, bool deep = false) where T : XdslElement
	{
		ThrowerHelper.ThrowIfArgumentNull(nameof(node), node);
		ThrowerHelper.ThrowIfArgumentNull(nameof(query), query);

		return new WhereQuery<T>(query, deep).Query(node);
	}

	/// <summary>
	/// Perform a find query on the node.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="query"></param>
	/// <param name="deep"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static XdslElement? Find(this XdslNode node, QueryAction<XdslElement> query, bool deep = false)
	{
		return Find<XdslElement>(node, query, deep);
	}

	/// <summary>
	/// Perform a where query on the node.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="query"></param>
	/// <param name="deep"></param>
	/// <returns></returns>
	public static IEnumerable<XdslElement> Where(this XdslNode node, QueryAction<XdslElement> query, bool deep = false)
	{
		return Where<XdslElement>(node, query, deep);
	}
}