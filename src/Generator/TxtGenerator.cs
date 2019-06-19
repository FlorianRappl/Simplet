using System.Collections.Generic;
using System.Linq;
using Simplet.Options;

namespace Simplet.Generator
{
    internal class TxtGenerator : IGenerator
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _files;

        public TxtGenerator(IEnumerable<IGeneratedClass> files)
        {
            _files = files.SelectMany(cls => cls.ResourceStrings);
        }

        public IEnumerable<IGeneratedFile> Generate(SimpletOptions options)
        {
            if (options.Sources.Any(m => m.UseResourceString))
            {
                yield return new TextFile("_ResourceStringExtensions.cs", $@"
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace {options.ProjectName}
{{
    internal static class _ResourceStringExtensions
    {{
        private static readonly IDictionary<string, string> _cache = new Dictionary<string, string>();

        public static string GetManifestResourceString(this string name)
        {{
            if (!_cache.TryGetValue(name, out var value))
            {{
                var fullName = string.Concat(typeof(_ResourceStringExtensions).Namespace, ""."", name);

                using (var stream = typeof(_ResourceStringExtensions).Assembly.GetManifestResourceStream(fullName))
                {{
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {{
                        value = reader.ReadToEnd();
                    }}
                }}

                _cache[name] = value;
            }}

            return value;
        }}
    }}
}}");
            }

            foreach (var file in _files)
            {
                yield return new TextFile(file.Key, file.Value);
            }
        }
    }
}
