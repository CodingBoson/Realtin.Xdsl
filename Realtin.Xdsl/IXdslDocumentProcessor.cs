namespace Realtin.Xdsl;

/// <summary>
/// Provides an <see langword="interface"/> for XDSL document processors.
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public interface IXdslDocumentProcessor<in TOptions> where TOptions : IXdslDocumentOptions<TOptions>
{
	/// <summary>
	/// Processes the specified <paramref name="document"/>.
	/// </summary>
	/// <param name="document"></param>
	/// <param name="options"></param>
	void Process(XdslDocument document, TOptions options);

	/// <summary>
	/// Processes the specified <paramref name="element"/>.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="options"></param>
	void Process(XdslElement element, TOptions options);
}