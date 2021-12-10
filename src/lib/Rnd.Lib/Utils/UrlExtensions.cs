using System;
using System.Collections.Generic;
using System.Text;

using Rnd.Lib.Extensions;

namespace Rnd.Lib.Utils
{
    public class UriEx
    {
        public static bool IsValidUrl(string url)
            => IsValid(url, new string[] { Uri.UriSchemeHttp, Uri.UriSchemeHttps });

        public static bool IsValid(string url, params string[] schemes)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                if (schemes.Length == 0)
                {
                    return true;
                }

                return schemes.Contains(uriResult.Scheme, StringComparison.OrdinalIgnoreCase);
            }
            return false;

            //bool passed = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
            //&& (uriResult.Scheme == Uri.UriSchemeHttp
            //  || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}