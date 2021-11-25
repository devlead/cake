using System.IO.Compression;

namespace Cake.Commands.Init.Extensions
{
    public static class ZipArchiveEntryExtensions
    {
        public static bool IsFile(this ZipArchiveEntry entry)
            => !IsDirectory(entry) && (entry.ExternalAttributes & 8) != 8;
        public static bool IsDirectory(this ZipArchiveEntry entry)
            => entry.FullName[^1] switch
               {
                   '/' => true,
                   '\\' => true,
                   _ => false
               }
               ||
               (entry.ExternalAttributes & 16) == 16;
    }
}