namespace Realtin.Xdsl.NamingConventions;

/// <summary>
/// Provides an <see langword="interface"/> for naming conventions.
/// </summary>
public interface INamingConvention
{
	/// <summary>
	/// Applies this naming convention on the specified input.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	string Apply(string input);
}