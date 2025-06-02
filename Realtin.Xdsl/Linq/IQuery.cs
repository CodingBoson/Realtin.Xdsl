namespace Realtin.Xdsl.Linq;

/// <summary>
/// Provides an <see langword="interface"/> for XDSL Linq queries.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<out TResult>
{
	/// <summary>
	/// Run this query.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	TResult? Query(XdslNode node);
}