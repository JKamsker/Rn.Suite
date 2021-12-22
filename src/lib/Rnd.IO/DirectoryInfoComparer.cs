using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rnd.IO
{
    public class DirectoryInfoComparer : IEqualityComparer<DirectoryInfo>
    {
        public static readonly DirectoryInfoComparer Instance = new DirectoryInfoComparer();
        public bool Equals(DirectoryInfo x, DirectoryInfo y)
        {
            return x?.Name.Equals(y?.Name) == true;
        }

        public int GetHashCode(DirectoryInfo obj)
        {
            return obj?.Name.GetHashCode() ?? obj.GetHashCode();
        }
    }
}