namespace Realtin.Xdsl.Serialization
{
    public static class XdslSerializerExtensions
    {
        /// <summary>
        /// Converts this <see cref="XdslDocument"/> representing an XDSL object into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="XdslSerializerException"></exception>
        public static T? Deserialize<T>(this XdslDocument document, XdslSerializerOptions? options = default)
        {
            return XdslSerializer.Deserialize<T>(document, options);
        }

        /// <summary>
        /// Converts this <see cref="XdslElement"/> representing an XDSL object into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="XdslSerializerException"></exception>
        public static T? Deserialize<T>(this XdslElement element, XdslSerializerOptions? options = default)
        {
            return XdslSerializer.Deserialize<T>(element, options);
        }

        /// <summary>
        /// Converts this <see cref="XdslDocument"/> representing an XDSL object into an instance of type <typeparamref name="T"/> using the provided <paramref name="serializer"/>.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="serializer"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="XdslSerializerException"></exception>
        public static T? Deserialize<T>(this XdslDocument document, XdslSerializer<T> serializer, XdslSerializerOptions? options = default)
        {
            return XdslSerializer.Deserialize(document, serializer, options);
        }

		/// <summary>
		/// Converts this <see cref="XdslElement"/> representing an XDSL object into an instance of type <typeparamref name="T"/> using the provided <paramref name="serializer"/>.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="serializer"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		/// <exception cref="XdslSerializerException"></exception>
		public static T? Deserialize<T>(this XdslElement element, XdslSerializer<T> serializer, XdslSerializerOptions? options = default)
        {
            return XdslSerializer.Deserialize(element, serializer, options);
        }
    }
}