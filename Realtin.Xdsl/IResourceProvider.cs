using System.IO;
using System.Threading.Tasks;

namespace Realtin.Xdsl;

public interface IResourceProvider
{
	Stream GetResourceStream(string path);

	byte[] GetResourceBytes(string path);

	Task<byte[]> GetResourceBytesAsync(string path);
}