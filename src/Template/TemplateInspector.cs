namespace Simplet.Template
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class TemplateInspector
    {
        private readonly string _raw;

        public TemplateInspector(string raw) => _raw = raw;

        public IEnumerable<TemplateReplacement> GetReplacements(Regex findPlaceholders)
        {
            var matches = findPlaceholders.Matches(_raw);
            var replacements = new List<TemplateReplacement>();

            foreach (Match match in matches)
            {
                replacements.Add(new TemplateReplacement
                {
                    Start = match.Index,
                    End = match.Index + match.Length,
                    Name = match.Groups[1].Value,
                    Placeholder = match.Value,
                });
            }

            replacements.Add(new TemplateReplacement
            {
                Start = _raw.Length,
                End = _raw.Length,
                Name = string.Empty,
                Placeholder = string.Empty,
            });

            return replacements;
        }

        public string GetTemplateString(IEnumerable<TemplateReplacement> replacements, IDictionary<string, string> modelNames, string prefix = null)
        {
            var sb = new StringBuilder();
            var previous = 0;

            foreach (var replacement in replacements)
            {
                var part = _raw.Substring(previous, replacement.Start - previous);
                var name = replacement.Name;

                foreach (var chr in part)
                {
                    switch (chr)
                    {
                        case '"':
                        case '{':
                        case '}':
                            sb.Append(chr).Append(chr);
                            break;
                        default:
                            sb.Append(chr);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(name) && modelNames.TryGetValue(name, out var identifier))
                {
                    var ident = (prefix ?? string.Empty) + identifier;
                    sb.Append("{" + ident + " ?? @\"" + replacement.Placeholder + "\"}");
                }

                previous = replacement.End;
            }

            return sb.ToString();
        }
    }
}
