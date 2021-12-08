using System;
using System.IO;

namespace Rnd.IO.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo AsDirectoryInfo(this string path)
        {
            return new DirectoryInfo(path);
        }

        public static DirectoryInfo EnsureCreated(this DirectoryInfo directoryInfo)
        {
            directoryInfo.Create();
            return directoryInfo;
        }

        public static DirectoryInfo ThrowIfNotExists(this DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                throw new IOException($"Directory not found! ({directoryInfo.FullName})");
            }

            return directoryInfo;
        }

        public static FileInfo GetNextFreeFile(this DirectoryInfo directoryInfo, Func<int, string> generator)
        {
            for (int i = 0; ; i++)
            {
                var cDir = directoryInfo.GetFile(generator(i));
                if (!cDir.Exists)
                {
                    return cDir;
                }
            }
        }

        public static DirectoryInfo GetNextFreeSubDirectory(this DirectoryInfo directoryInfo, Func<int, string> generator)
        {
            for (int i = 0; ; i++)
            {
                var cDir = directoryInfo.GetDirectory(generator(i));
                if (!cDir.Exists)
                {
                    return cDir;
                }
            }
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directoryInfo, params string[] subdirectoryName)
        {
            var path = Path.Combine(directoryInfo.FullName, Path.Combine(subdirectoryName));
            return new DirectoryInfo(path);
        }

        public static FileInfo GetFile(this DirectoryInfo directoryInfo, params string[] fileName)
        {
            var path = Path.Combine(directoryInfo.FullName, Path.Combine(fileName));
            return new FileInfo(path);
        }

        public static DirectoryInfo EnsureEmpty(this DirectoryInfo directoryInfo) => directoryInfo
            .EnsureDeleted(true)
            .EnsureCreated();
        
        public static DirectoryInfo EnsureDeleted(this DirectoryInfo directoryInfo, bool recursive = false)
        {
            try
            {
                if (directoryInfo.Exists)
                {
                    directoryInfo.Refresh();
                    directoryInfo.Delete(recursive);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            directoryInfo.Refresh();
            return directoryInfo;
        }
        
        
    }
}