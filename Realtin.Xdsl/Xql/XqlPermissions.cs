using System;

namespace Realtin.Xdsl.Xql
{
	/// <summary>
	/// XQL Permissions.
	/// </summary>
	[Flags]
	public enum XqlPermissions : byte
	{
		/// <summary>
		/// No access.
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Read only access.
		/// </summary>
		Read = 0x2,

		/// <summary>
		/// Write only access.
		/// </summary>
		Write = 0x4,

		/// <summary>
		/// Read and Write access.
		/// </summary>
		ReadWrite = Read | Write
	}
}