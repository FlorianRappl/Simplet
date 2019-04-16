namespace Simplet.Options
{
    using Newtonsoft.Json;

    public class TemplateSectionOptions
    {
        [JsonProperty("format")]
        public string SectionFormat { get; set; } = string.Empty;

        [JsonProperty("title")]
        [JsonRequired]
        public string Title { get; set; }
    }
}
