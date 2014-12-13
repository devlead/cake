﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Cake.Core.IO
{
    /// <summary>
    /// Contains extension methods for <see cref="IFile"/>.
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Opens the file using the specified options.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        public static Stream Open(this IFile file, FileMode mode)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            return file.Open(mode, 
                mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite, 
                FileShare.None);
        }

        /// <summary>
        /// Opens the file using the specified options.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        public static Stream Open(this IFile file, FileMode mode, FileAccess access)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            return file.Open(mode, access, FileShare.None);
        }

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        public static Stream OpenRead(this IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Opens the file for writing.
        /// If the file already exists, it will be overwritten.
        /// </summary>
        /// <param name="file">The file to be opened.</param>
        /// <returns>A <see cref="Stream"/> to the file.</returns>
        public static Stream OpenWrite(this IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            return file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
        }

        /// <summary>
        /// Enumerates line in file
        /// </summary>
        /// <param name="file">The file to be read from.</param>
        /// <param name="encoding">The encoding that is applied to the content of the file.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of file line content</returns>
        public static IEnumerable<string> ReadLines(this IFile file, System.Text.Encoding encoding)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (!file.Exists)
            {
                throw new FileNotFoundException("Solution doesn't exist", file.Path.FullPath);
            }
            return System.IO.File.ReadLines(file.Path.FullPath, encoding);
        }
    }
}
