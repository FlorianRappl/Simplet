namespace Simplet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Simplet.Options;
    using Simplet.Template;

    internal partial class CsGenerator : IGenerator
    {
        public IEnumerable<IGeneratedFile> Generate(SimpletOptions options)
        {
            var cwd = Environment.CurrentDirectory;
            var interfaceProperties = new HashSet<string>();
            var templateId = 1;
            var allFiles = Directory.GetFiles(cwd, "*", SearchOption.AllDirectories)
                .Select(m => m.Remove(0, cwd.Length + 1))
                .ToArray();

            foreach (var source in options.Sources)
            {
                var findPlaceholders = new Regex(source.PlaceholderFormat, RegexOptions.Compiled);
                var glob = new GlobMatcher(source.IncludePaths, source.ExcludePaths);
                var files = allFiles.Where(file => glob.IsIncluded(file)).ToArray();
                var commonStart = files.FindCommonStart();
                var sourceName = string.Empty;

                var fileMapping = files.ToDictionary(
                    file => file.RemoveCommon(commonStart),
                    file => new TemplateInspector(File.ReadAllText(file))
                );

                var groups = fileMapping.Keys.Segmentize(source.ParameterType);

                if (source.Sections.Any())
                {
                    var ident = (source.TemplateName ?? $"Template_{templateId++}").ToCsharpIdent();
                    sourceName = $"I{ident}";
                    yield return GenerateTemplateInterface(options, source, sourceName);
                }

                foreach (var group in groups)
                {
                    var key = string.IsNullOrEmpty(group.Key) ? "All" : group.Key;
                    var name = key.ToCsharpIdent();
                    var modelCls = name + "Model";
                    var templateCls = name + "Template";
                    var result = GetTemplates(group, fileMapping, findPlaceholders);

                    interfaceProperties.UnionWith(result.Identifiers.Select(m => m.Value));

                    yield return GenerateModel(options, source, modelCls, result.Identifiers);
                    yield return GenerateTemplate(options, source, modelCls, templateCls, result.Templates, sourceName);
                }
            }

            yield return GeneratePropertyInterfaces(options, interfaceProperties);
        }

        private struct TemplateResult
        {
            public Dictionary<string, string> Identifiers;
            public Dictionary<string, string> Templates;
        }

        private static TemplateResult GetTemplates(IGrouping<string, string> group, IDictionary<string, TemplateInspector> fileMapping, Regex findPlaceholders)
        {
            var templates = new Dictionary<string, string>();
            var identifiers = new Dictionary<string, string>();

            foreach (var file in group)
            {
                var tmpl = fileMapping[file];
                var repl = tmpl.GetReplacements(findPlaceholders);
                MergeIdentifiers(repl, identifiers);
                templates.Add(file, tmpl.GetTemplateString(repl, identifiers, "model."));
            }

            return new TemplateResult
            {
                Identifiers = identifiers,
                Templates = templates,
            };
        }

        private static void MergeIdentifiers(IEnumerable<TemplateReplacement> replacements, Dictionary<string, string> target)
        {
            foreach (var replacement in replacements)
            {
                if (!string.IsNullOrEmpty(replacement.Name) && !target.ContainsKey(replacement.Name))
                {
                    var ident = replacement.Name.ToCsharpIdent();

                    if (target.ContainsValue(ident))
                    {
                        var v = 2;

                        while (target.ContainsValue($"{ident}_{v}"))
                        {
                            v++;
                        }

                        ident = $"{ident}_{v}";
                    }

                    target.Add(replacement.Name, ident);
                }
            }
        }

        private static IDictionary<string, string> GetIdentifiers(IEnumerable<TemplateReplacement> replacements)
        {
            var model = new Dictionary<string, string>();

            MergeIdentifiers(replacements, model);

            return model;
        }
    }
}
