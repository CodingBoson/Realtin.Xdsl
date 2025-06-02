using System.Collections.Generic;

namespace Realtin.Xdsl.Serialization;

internal static class CachedXdslDocumentLoader
{
	internal static XdslDocumentOptions DocumentOptions { get; }
		= new XdslDocumentOptions() {
			CommentHandling = XdslCommentHandling.Ignore,
			TagHandling = XdslTagHandling.Ignore,
		};

	private static readonly object _lock = new();

	private static readonly Dictionary<string, XdslDocument> _cache = [];

	public static XdslDocument Create(string xdsl)
	{
		lock (_lock) {
			if (!_cache.TryGetValue(xdsl, out var cache)) {
				cache = XdslDocument.Create(xdsl, DocumentOptions);

				_cache.Add(xdsl, cache);
			}

			return cache;
		}
	}
}