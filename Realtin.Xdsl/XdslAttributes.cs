using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl
{
	/// <summary>
	/// Commonly used xdsl attributes.
	/// </summary>
	public static class XdslAttributes
	{
		/// <summary>
		/// Used by the 
		/// <see cref="XdslSerializeReferenceAttribute"/> 
		/// to store the type info on the xdsl element.
		/// </summary>
		public const string Class = "class";

        /// <summary>
        /// Used to determine whether a collection is empty or null.
        /// <para>True if the collection is null, False if the collection is empty.</para>
        /// </summary>
        public const string Null = "null";

        /// <summary>
        /// Used to store the encoding used to encode and decode a <see cref="string"/>.
        /// </summary>
        public const string Encoding = "encoding";
    }
}