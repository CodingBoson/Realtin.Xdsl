namespace Realtin.Xdsl.Serialization
{
	/// <summary>
	/// Implement this interface to receive serialization and deserialization callbacks.
	/// </summary>
	public interface IXdslSerializationCallbackReceiver
	{
		/// <summary>
		/// Implement this function to receive a callback before XDSL serializes your object.
		/// </summary>
		void OnBeforeSerialize();

		/// <summary>
		/// Implement this function to receive a callback after XDSL deserializes your object.
		/// </summary>
		void OnAfterDeserialize();
	}
}