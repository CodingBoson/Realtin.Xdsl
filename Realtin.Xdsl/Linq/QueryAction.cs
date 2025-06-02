namespace Realtin.Xdsl.Linq;

/// <summary>
/// Represents an XDSL Linq query <see langword="delegate"/>.
/// </summary>
/// <typeparam name="TNode"></typeparam>
/// <param name="node"></param>
/// <returns></returns>
public delegate bool QueryAction<in TNode>(TNode node) where TNode : XdslNode;