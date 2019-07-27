using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace PhotosForGrandpa.WPF.Extensions
{
    public static class ZipArchiveEntryExtensions
    {
        public static IEnumerable<ZipArchiveEntry> GetFilesWithExtension(
            this IEnumerable<ZipArchiveEntry> zipArchiveEntries,
            params string[] allowedExtensions)
        {
            foreach (var file in zipArchiveEntries)
            {
                foreach (var extension in allowedExtensions)
                {
                    if (file.FullName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
