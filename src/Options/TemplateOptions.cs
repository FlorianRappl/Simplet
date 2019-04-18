namespace Simplet.Options
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class TemplateOptions
    {
        /// <summary>
        /// Gets or sets the optional name of the given templates. Used
        /// to derive the common interface, if necessary.
        /// </summary>
        [JsonProperty("name")]
        public string TemplateName { get; set; } = null;

        /// <summary>
        /// Gets or sets the placeholder format. Must be a regular expression.
        /// The regular expression has to contain a single group (name of the
        /// placeholder).
        /// </summary>
        [JsonProperty("placeholder")]
        public string PlaceholderFormat { get; set; } = "%([A-Za-z]{1,128})%";

        /// <summary>
        /// Gets or sets the included paths. Every path can be a standard glob
        /// pattern.
        /// </summary>
        [JsonProperty("includes")]
        [JsonRequired]
        public List<string> IncludePaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the excluded paths. Every path can be a standard glob
        /// pattern.
        /// </summary>
        [JsonProperty("excludes")]
        public List<string> ExcludePaths { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the parameterization strategy.
        /// </summary>
        [JsonProperty("parameter")]
        public TemplateParameterType ParameterType { get; set; } = TemplateParameterType.None;

        /// <summary>
        /// Gets or sets the namespace. If none is given the name of the project
        /// is used as the namespace.
        /// </summary>
        [JsonProperty("namespace")]
        public string Namespace { get; set; } = null;

        /// <summary>
        /// Gets or sets the sections if any. Sections split the full template into multiple
        /// accessible parts.
        /// </summary>
        [JsonProperty("sections")]
        public List<TemplateSectionOptions> Sections { get; set; } = new List<TemplateSectionOptions>();
    }
}
