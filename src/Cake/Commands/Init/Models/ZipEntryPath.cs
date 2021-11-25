using System.IO.Compression;
using Cake.Core.IO;

namespace Cake.Commands.Init.Models
{
    public class ZipEntryPath
    {
        public ZipArchiveEntry Entry { get; }
        public bool IsFile { get; }
        public Path Path { get; }

        public ZipEntryPath(ZipArchiveEntry entry, bool isFile, Path path)
        {
            Entry = entry;
            IsFile = isFile;
            Path = path;
        }
    }
}