namespace Simplet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Simplet.Options;

    internal partial class CsGenerator : IGenerator
    {
        private static readonly Regex _captureVariables = new Regex("(?<!{){(?<name>[^{]+)}(?!})", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static IGeneratedFile GeneratePropertyInterfaces(SimpletOptions options, IEnumerable<string> interfaceProperties)
        {
            var content = string.Join(string.Empty, interfaceProperties.Select(propName => $@"
    public interface {propName.ToCsharpInterface()}
    {{
       string {propName} {{ get; set; }}
    }}
"));
            return new TextFile($"AllInterfaces.cs", $@"namespace {options.ProjectName}
{{{content}}}");
        }

        private static IGeneratedFile GenerateTemplateInterface(SimpletOptions options, TemplateOptions source, string sourceIf)
        {
            var properties = new List<string>();

            foreach (var section in source.Sections)
            {
                properties.Add($@"
        string {section.Title.ToCsharpIdent()} {{ get; }}
");
            }

            var content = string.Join(string.Empty, properties);
            return new TextFile($"{sourceIf}.cs", $@"namespace {options.ProjectName}
{{
    public interface {sourceIf}
    {{{content}    }}
}}");
        }

        private static IGeneratedFile GenerateModel(SimpletOptions options, TemplateOptions source, string cls, IDictionary<string, string> idents)
        {
            var ifs = string.Join(", ", idents.Select(m => m.Value.ToCsharpInterface()));
            var properties = string.Join(string.Empty, idents.Select(m => $@"
        /// <summary>Gets or sets the value for replacing '{m.Key}'.</summary>
        public string {m.Value} {{ get; set; }}
"));
            return new TextFile($"{cls}.cs", $@"namespace {source.Namespace ?? options.ProjectName}
{{
    public class {cls} : {ifs}
    {{{properties}    }}
}}");
        }

        private static IGeneratedFile GenerateTemplate(SimpletOptions options, TemplateOptions source, string modelCls, string templateCls, Dictionary<string, string> templates, string sourceIf)
        {
            var resources = new Dictionary<string, string>();
            var hasSections = source.Sections.Any();
            var type = source.ParameterType != TemplateParameterType.None ? "type" : "";
            var templateImpl = "return null;";
            var isStatic = hasSections ? "sealed" : "static";
            var response = hasSections ? templateCls : "string";
            var headerImpl = string.Empty;
            var resInterface = string.IsNullOrEmpty(sourceIf) ? "" : $" : {sourceIf}";
            var parameters = new List<string>
            {
                $"{modelCls} model",
            };

            if (string.IsNullOrEmpty(type))
            {
                templateImpl = $@"return $@""{templates.Values.Single()}"";";
            }
            else if (source.UseResourceString)
            {
                parameters.Add(!string.IsNullOrEmpty(type) ? $"string {type}" : "");
                var cases = templates.Select(m =>
                {
                    var pt = m.Key.ToCsharpIdent(source.ParameterType);
                    var value = m.Value;
                    var args = new List<string>();
                    var matches = _captureVariables.Matches(value);

                    foreach (var match in matches.Reverse())
                    {
                        var group = match.Groups[1];
                        args.Insert(0, group.Value);
                        value = $"{value.Substring(0, group.Index)}{matches.Count - args.Count}{value.Substring(group.Index + group.Length)}"; 
                    }

                    var name = $"{templateCls}_{pt}.txt";
                    resources.Add(name, value.Replace("\"\"", "\""));
                    return $@"
                case ""{pt}"":
                    return new {templateCls}(string.Format(""{name}"".GetManifestResourceString(), {string.Join(", ", args)}));
";
                });
                templateImpl = $@"switch ({type})
            {{{string.Join(string.Empty, cases)}                    }}
            return null;";
            }
            else
            {
                parameters.Add(!string.IsNullOrEmpty(type) ? $"string {type}" : "");
                var (individual, common) = templates.Optimize();
                var decls = common.Select(m => $@"var {m.Key} = $@""{m.Value}"";");
                var cases = individual.Select(m => $@"
                case ""{m.Key.ToCsharpIdent(source.ParameterType)}"":
                    return new {templateCls}($@""{m.Value}"");
");
                templateImpl = $@"{string.Join(Environment.NewLine, decls)}

            switch ({type})
            {{{string.Join(string.Empty, cases)}                    }}
            return null;";
            }

            var templateParameter = string.Join(", ", parameters);
            var templateArguments = string.Join(", ", parameters.Select(m => m.Split(' ').Last()));
            var result = $"GetTemplate({templateArguments})";

            var lines = new List<string>
            {
                $@"
        public static {response} Default({templateParameter}) => {result};

        private static {response} GetTemplate({templateParameter})
        {{
            {headerImpl}
            {templateImpl}
        }}
",
            };

            if (!string.IsNullOrEmpty(type))
            {
                var identStrings = new List<string>();

                foreach (var key in templates.Keys)
                {
                    var ident = key.ToCsharpIdent(source.ParameterType);
                    lines.Add($@"
        public static {response} {ident}({parameters.First()}) => {result.Replace(type, $"\"{ident}\"")};
");
                    identStrings.Add($"\"{ident}\"");
                }

                lines.Add($@"
        public static string[] AvailableTypes => new [] {{ {string.Join(", ", identStrings)} }};
");
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
            
            return new TextClass($"{templateCls}.cs", $@"namespace {source.Namespace ?? options.ProjectName}
{{
    public {isStatic} class {templateCls}{resInterface}
    {{{string.Join(string.Empty, lines)}    }}
}}", resources);
        }
    }
}
