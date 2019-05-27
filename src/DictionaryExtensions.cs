namespace Simplet
{
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static (Dictionary<string, string> individual, Dictionary<string, string> common) Optimize(this Dictionary<string, string> templates)
        {
            var individual = new Dictionary<string, string>();
            var common = new Dictionary<string, string>();
            var start = templates.Values.FindCommonStart();
            var end = templates.Values.FindCommonEnd();
            var removed = start.Length + end.Length;

            common.Add("start", start);
            common.Add("end", end);

            foreach (var template in templates)
            {
                var value = template.Value;
                var part = value.Substring(start.Length, value.Length - removed);
                individual.Add(template.Key, string.Concat("{start}", part, "{end}"));
            }

            return (individual, common);
        }
    }
}
