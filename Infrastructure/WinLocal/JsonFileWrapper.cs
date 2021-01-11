using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.IO;
using System.Text;
using System;

namespace Infrastructure
{
    public class JsonFileWrapper
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true,
            IgnoreNullValues = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = false,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip, //TODO: Allowにできないか？
            AllowTrailingCommas = true,
        };
        private static readonly string APP_DATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public readonly string filePath = Path.Combine(APP_DATA_PATH, "ProcessMarker", "config.json");

        protected string CreateDirectory()
        {
            var directoryName = Path.GetDirectoryName(filePath);
            if(!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            return directoryName;
        }

        protected string Serialize<T>(T item)
        {
            return JsonSerializer.Serialize(item, jsonSerializerOptions);
        }

        protected T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }

        protected string ReadTextFile()
        {
            using(StreamReader sr = new StreamReader(filePath, new UTF8Encoding(true)))
            {
                return sr.ReadToEnd();
            }
        }

        protected string WriteTextFile(string text)
        {
            using(StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                sw.Write(text);
            }
            return text;
        }
    }
}
