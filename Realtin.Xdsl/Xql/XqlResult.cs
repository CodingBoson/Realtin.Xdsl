namespace Realtin.Xdsl.Xql;

/// <summary>
/// Represents an XQL query result.
/// </summary>
public readonly struct XqlResult
{
	/// <summary>
	/// Returns a value that indicates whether the XQL operation resulted in success.
	/// </summary>
	public bool Success { get; }

	/// <summary>
	/// Returns a value that explains why the XQL operation failed.
	/// </summary>
	/// <returns>An error message if the operation failed, otherwise an empty string.</returns>
	public string Message { get; }

	/// <summary>
	/// The value returned by an XQL operation
	/// </summary>
	public object? Data { get; } = null;

	/// <summary>
	/// Returns a value that indicates whether this <see cref="XqlResult"/> has any data.
	/// </summary>
	public bool HasData => Data != null;

	/// <summary>
	/// Initialize a new instance of the <see cref="XqlResult"/> structure.
	/// </summary>
	/// <param name="success"></param>
	/// <param name="message"></param>
	/// <param name="data"></param>
	public XqlResult(bool success, string message, object? data)
	{
		Success = success;
		Message = message;
		Data = data;
	}

	/// <summary>
	/// Returns a value that indicates whether this result's data represent 
	/// an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public bool Is<T>() where T : class => Data is T;

	/// <summary>
	/// Casts this result's data to a type specified by the <typeparamref name="T"/> parameter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T? As<T>() where T : class => Data as T;

	/// <summary>
	/// Casts this result's data to a type specified by the <typeparamref name="T"/> parameter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T Cast<T>() where T : class =>
		// Allow the user to specify if the type is nullable.
		(T)Data!;

	/// <summary>
	/// Creates a failed <see cref="XqlResult"/> structure.
	/// </summary>
	/// <param name="message"></param>
	/// <returns></returns>
	public static XqlResult Failed(string message) => new(false, message, null);

	/// <summary>
	/// Creates a succeeded <see cref="XqlResult"/> structure.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static XqlResult Succeed(object? data) => new(true, string.Empty, data);
}