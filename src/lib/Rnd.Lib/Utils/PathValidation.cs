using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Rnd.Lib.Utils
{
    public static class PathValidation
    {
        private static string _pathValidatorExpression = "^[^" + string.Join("", Array.ConvertAll(Path.GetInvalidPathChars(), x => Regex.Escape(x.ToString()))) + "]+$";
        private static Regex _pathValidator = new Regex(_pathValidatorExpression, RegexOptions.Compiled);

        private static string _fileNameValidatorExpression = "^[^" + string.Join("", Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString()))) + "]+$";
        private static Regex _fileNameValidator = new Regex(_fileNameValidatorExpression, RegexOptions.Compiled);

        private static string _pathCleanerExpression = "[" + string.Join("", Array.ConvertAll(Path.GetInvalidPathChars(), x => Regex.Escape(x.ToString()))) + "]";
        private static Regex _pathCleaner = new Regex(_pathCleanerExpression, RegexOptions.Compiled);

        private static string _fileNameCleanerExpression = "[" + string.Join("", Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString()))) + "]";
        private static Regex _fileNameCleaner = new Regex(_fileNameCleanerExpression, RegexOptions.Compiled);

        public static bool ValidatePath(string path)
        {
            return _pathValidator.IsMatch(path);
        }

        public static bool ValidateFileName(string fileName)
        {
            return _fileNameValidator.IsMatch(fileName);
        }

        public static string CleanPath(string path)
        {
            return _pathCleaner.Replace(path, "_");
        }

        public static string CleanFileName(string fileName)
        {
            return _fileNameCleaner.Replace(fileName, "_");
        }
    }
}
