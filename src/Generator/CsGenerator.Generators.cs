namespace Simplet.Generator
{
    using System.Collections.Generic;
    using System.Linq;
    using Simplet.Options;

    internal partial class CsGenerator : IGenerator
    {
        private static IGeneratedFile GenerateModel(SimpletOptions options, TemplateOptions source, string cls, IDictionary<string, string> idents)
        {
            var properties = string.Join(string.Empty, idents.Select(m => $@"
        /// <summary>Gets or sets the value for replacing '{m.Key}'.</summary>
        public string {m.Value} {{ get; set; }}
"));
            return new TextFile($"{cls}.cs", $@"namespace {source.Namespace ?? options.ProjectName}
{{
    public class {cls}
    {{{properties}    }}
}}");
        }

        private static IGeneratedFile GenerateTemplate(SimpletOptions options, TemplateOptions source, string modelCls, string templateCls, Dictionary<string, string> templates)
        {
            var hasSections = source.Sections.Any();
            var type = source.ParameterType != TemplateParameterType.None ? "type" : "";
            var templateImpl = "return null;";
            var isStatic = hasSections ? "sealed" : "static";
            var response = hasSections ? templateCls : "string";
            var parameters = new List<string>
            {
                $"{modelCls} model",
            };

            if (string.IsNullOrEmpty(type))
            {
                templateImpl = $@"return $@""{templates.Values.Single()}"";";
            }
            else
            {
                parameters.Add(!string.IsNullOrEmpty(type) ? $"string {type}" : "");
                var cases = templates.Select(m => $@"
                case ""{m.Key.ToCsharpIdent(source.ParameterType)}"":
                    return $@""{m.Value}"";
");
                templateImpl = $@"switch ({type})
            {{{string.Join(string.Empty, cases)}                    }}
            return null;";
            }

            var templateParameter = string.Join(", ", parameters);
            var templateArguments = string.Join(", ", parameters.Select(m => m.Split(' ').Last()));
            var result = hasSections ? $@"new {templateCls}(GetTemplate({templateArguments}))" : $"GetTemplate({templateArguments})";

            var lines = new List<string>
            {
                $@"
        public static {response} Default({templateParameter}) => {result};

        private static string GetTemplate({templateParameter})
        {{
            {templateImpl}
        }}
",
            };

            if (!string.IsNullOrEmpty(type))
            {
                foreach (var key in templates.Keys)
                {
                    var ident = key.ToCsharpIdent(source.ParameterType);
                    lines.Add($@"
        public static {response} {ident}({parameters.First()}) => {result.Replace(type, $"\"{ident}\"")};
");
                }
            }

            if (hasSections)
            {
                lines.Add($@"
        private readonly string _content;

        public {templateCls}(string content) => _content = content;

        private string Cut(string format) => System.Text.RegularExpressions.Regex.Match(_content, format, System.Text.RegularExpressions.RegexOptions.Singleline).Groups[1].Value;
");

                foreach (var section in source.Sections)
                {
                    lines.Add($@"
        public string {section.Title.ToCsharpIdent()} => Cut(""{section.SectionFormat}"");
");
                }

                if (!source.Sections.Any(m => m.Title == "Full"))
                {
                    lines.Add($@"
        public string Full => Cut(""(.*)"");
");
                }
            }
            
            return new TextFile($"{templateCls}.cs", $@"namespace {source.Namespace ?? options.ProjectName}
{{
    public {isStatic} class {templateCls}
    {{{string.Join(string.Empty, lines)}    }}
}}");
        }
    }
}
