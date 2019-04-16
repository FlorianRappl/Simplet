namespace Simplet.Options
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public class SimpletOptions
    {
        [JsonProperty("target")]
        public string TargetDirectory { get; set; } = "generated";

        [JsonProperty("name")]
        public string ProjectName { get; set; } = "GeneratedTemplates";

        [JsonProperty("framework")]
        public string TargetFramework { get; set; } = "netstandard2.0";

        [JsonProperty("sources")]
        [JsonRequired]
        public List<TemplateOptions> Sources { get; set; } = new List<TemplateOptions>();

        public static SimpletOptions ReadFrom(string path)
        {
            var serializer = new JsonSerializer();

            using (var fs = File.OpenText(path))
            {
                return serializer.Deserialize(fs, typeof(SimpletOptions)) as SimpletOptions;
            }
        }
    }
}
