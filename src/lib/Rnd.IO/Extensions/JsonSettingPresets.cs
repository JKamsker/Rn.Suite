using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rnd.IO.Extensions
{
    internal static class JsonSettingPresets
    {
        static JsonSettingPresets()
        {
            Indented = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            Indented.Converters.Add(new StringEnumConverter());

            NonIndented = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            NonIndented.Converters.Add(new StringEnumConverter());
        }

        public static JsonSerializerSettings Indented { get; }
        public static JsonSerializerSettings NonIndented { get; }
    }
}