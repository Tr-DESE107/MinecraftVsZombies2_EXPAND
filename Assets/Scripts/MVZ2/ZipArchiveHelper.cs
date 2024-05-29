using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVZ2
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

    }
}
