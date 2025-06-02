using System.IO;
using System.Threading.Tasks;

namespace Realtin.Xdsl;

public class FileResourceProvider(string basePath) : IResourceProvider
{
    public string BasePath { get; } = basePath;

    public Stream GetResourceStream(string path)
	{
		return File.OpenRead(Path.Combine(BasePath, path));
	}

    public byte[] GetResourceBytes(string path)
    {
		return File.ReadAllBytes(Path.Combine(BasePath, path));
    }

    public async Task<byte[]> GetResourceBytesAsync(string path)
	{
		return await File.ReadAllBytesAsync(Path.Combine(BasePath, path));
	}
}