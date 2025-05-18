using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using NGettext;

namespace MVZ2.IO
{
    public static class ZipArchiveHelper
    {
        public static string ReadString(this ZipArchiveEntry entry, Encoding encoding)
        {
            using (var zipStream = entry.Open())
            {
                using (var reader = new StreamReader(zipStream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static async Task<string> ReadStringAsync(this ZipArchiveEntry entry, Encoding encoding)
        {
            using (var zipStream = entry.Open())
            {
                using (var reader = new StreamReader(zipStream, encoding))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
        public static Catalog ReadCatalog(this ZipArchiveEntry entry, string language)
        {
            using var zipStream = entry.Open();
            {
                using var memory = new MemoryStream();
                {
                    zipStream.CopyTo(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    return new Catalog(memory, new CultureInfo(language));
                }
            }
        }
        public static async Task<Catalog> ReadCatalogAsync(this ZipArchiveEntry entry, string language)
        {
            using var zipStream = entry.Open();
            {
                using var memory = new MemoryStream();
                {
                    await zipStream.CopyToAsync(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    return new Catalog(memory, new CultureInfo(language));
                }
            }
        }
        public static byte[] ReadBytes(this ZipArchiveEntry entry)
        {
            using (var entryStream = entry.Open())
            {
                using (var memory = new MemoryStream())
                {
                    entryStream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }
        public static async Task<byte[]> ReadBytesAsync(this ZipArchiveEntry entry)
        {
            using (var entryStream = entry.Open())
            {
                using (var memory = new MemoryStream())
                {
                    await entryStream.CopyToAsync(memory);
                    return memory.ToArray();
                }
            }
        }
    }
}
