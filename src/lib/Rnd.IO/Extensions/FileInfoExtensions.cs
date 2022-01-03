using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Rnd.IO.Extensions
{
    public static class FileInfoExtensions
    {
        public static FileInfo ThrowIfNotExists(this FileInfo fileInfo)
        {
            fileInfo.Refresh();
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(fileInfo.FullName);
            }

            return fileInfo;
        }

        public static FileInfo GetFileInfo(this DirectoryInfo directoryInfo, string fileName)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        public static FileInfo EnsureParentCreated(this FileInfo fileInfo)
        {
            var parent = fileInfo.Directory;
            parent.EnsureCreated();
            fileInfo.Refresh();
            return fileInfo;
        }

        public static FileInfo EnsureDeleted(this FileInfo fileInfo)
        {
            try
            {
                fileInfo.Refresh();
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                fileInfo.Refresh();
            }

            return fileInfo;
        }

        public static string ReadAllText(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return String.Empty;
            }

            try
            {
                return File.ReadAllText(fileInfo.FullName);
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        public static string[] ReadAllLines(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return new string[0];
            }

            try
            {
                return File.ReadAllLines(fileInfo.FullName);
            }
            catch (Exception e)
            {
                return new string[0];
            }
        }

        public static byte[] ReadAllBytes(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return new byte[0];
            }

            try
            {
                return File.ReadAllBytes(fileInfo.FullName);
            }
            catch (Exception e)
            {
                return new byte[0];
            }
        }

        public static FileInfo WriteAllBytes(this FileInfo fileInfo, byte[] bytes)
        {
            File.WriteAllBytes(fileInfo.FullName, bytes);
            return fileInfo;
        }

        public static FileInfo WriteAllText(this FileInfo fileInfo, string content)
        {
            File.WriteAllText(fileInfo.FullName, content);
            return fileInfo;
        }

        public static async Task<FileInfo> WriteAllTextAsync(this FileInfo fileInfo, string content)
        {
            await File.WriteAllTextAsync(fileInfo.FullName, content);
            return fileInfo;
        }

        public static FileInfo WriteAllLines(this FileInfo fileInfo, IEnumerable<string> content)
        {
            File.WriteAllLines(fileInfo.FullName, content);
            return fileInfo;
        }

        public static FileInfo MoveTo(this FileInfo fileInfo, FileInfo targetFile)
        {
            fileInfo.MoveTo(targetFile.FullName);
            return targetFile;
        }


        public static FileInfo AsFileInfo(this string path)
        {
            return new FileInfo(path);
        }

        public static string GetFileNameWithoutExtension(this FileInfo fileInfo)
        {
            return Path.GetFileNameWithoutExtension(fileInfo.FullName);
        }

        public static FileInfo WriteJson(this FileInfo fileInfo, object obj, bool indented = false)
        {
            return fileInfo.WriteJson(obj, indented ? JsonSettingPresets.Indented : JsonSettingPresets.NonIndented);
        }

        public static FileInfo WriteJson(this FileInfo fileInfo, object obj, JsonSerializerSettings settings)
        {
            settings ??= JsonSettingPresets.Indented;
            fileInfo.WriteAllText(JsonConvert.SerializeObject(obj, settings));
            return fileInfo;
        }

        public static FileInfo WriteStream(this FileInfo fileInfo, Stream obj)
        {
            using var iStream = obj;
            using var oStream = fileInfo.OpenWrite();
            iStream.CopyTo(oStream);

            return fileInfo;
        }

        public static T ReadJson<T>(this FileInfo fileInfo, JsonSerializer serializer = null)
        {
            serializer ??= JsonSerializer.CreateDefault(null);

            using var reader = new JsonTextReader(new StringReader(fileInfo.ReadAllText()));
            return serializer.Deserialize<T>(reader);

            // return JsonConvert.DeserializeObject<T>(fileInfo.ReadAllText());
        }

        public static List<T> ReadJsonList<T>(this FileInfo fileInfo)
        {
            return fileInfo.ReadJson<List<T>>();
        }

        public static string GetRelativePathTo(this FileInfo file, DirectoryInfo directoryInfo)
        {
            if (!file.FullName.StartsWith(directoryInfo.FullName))
            {
                throw new ArgumentException("Unable to make relative path");
            }

            return file.FullName.Replace(directoryInfo.FullName, "").TrimStart('\\');
        }
    }
}