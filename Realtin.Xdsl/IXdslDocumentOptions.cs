namespace Realtin.Xdsl;

/// <summary>
/// Provides an <see langword="interface"/> for XDSL document options.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
public interface IXdslDocumentOptions<TSelf> : ICloneable<TSelf> where TSelf : IXdslDocumentOptions<TSelf>
{
}