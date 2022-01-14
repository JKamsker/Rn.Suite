using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rnd.IO.Extensions
{
    public static class FileSystemInfoExtensions
    {
        public static bool HasSubDirectories(this FileSystemInfo? fsInfo)
        {
            if (fsInfo is null)
            {
                return false;
            }

            var directory = fsInfo switch
            {
                FileInfo fInfo => fInfo.Directory,
                DirectoryInfo directoryInfo => directoryInfo,
                _ => throw new NotSupportedException()
            };

            return directory?.EnumerateDirectories().Any() == true;
        }
    }
}
