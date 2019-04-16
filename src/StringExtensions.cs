namespace Simplet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class StringExtensions
    {
        public static string FindCommonStart(this IEnumerable<string> strings)
        {
            var current = strings.FirstOrDefault();

            foreach (var str in strings)
            {
                var len = Math.Min(current.Length, str.Length);
                var i = 0;

                while (i < len)
                {
                    if (str[i] != current[i])
                    {
                        break;
                    }

                    i++;
                }

                if (i != current.Length)
                {
                    current = str.Substring(0, i);
                }
            }

            return current;
        }

        public static string FindCommonEnd(this IEnumerable<string> strings)
        {
            var current = strings.FirstOrDefault();

            foreach (var str in strings)
            {
                var cl = current.Length;
                var sl = str.Length;
                var len = Math.Min(cl, sl);
                var i = 0;

                while (i < len)
                {
                    if (str[sl - i - 1] != current[cl - i - 1])
                    {
                        break;
                    }

                    i++;
                }

                if (i != cl)
                {
                    current = str.Substring(sl - i);
                }
            }

            return current;
        }

        public static string RemoveCommon(this string str, string start = "", string end = "")
        {
            if (start.Length + end.Length < str.Length)
            {
                return str.Substring(start.Length, str.Length - end.Length - start.Length);
            }

            return str;
        }

        public static IEnumerable<IGrouping<string, string>> Segmentize(this IEnumerable<string> files, TemplateParameterType type)
        {
            switch (type)
            {
                case TemplateParameterType.File:
                    return files.GroupBy(Path.GetDirectoryName);
                case TemplateParameterType.Directory:
                    return files.GroupBy(Path.GetFileNameWithoutExtension);
                case TemplateParameterType.Extension:
                    return files.GroupBy(Path.GetExtension);
                case TemplateParameterType.None:
                default:
                    return files.GroupBy(m => m);
            }
        }

        public static string ToTitle(this string str) => str.Substring(0, 1) + str.Substring(1).ToLower();

        public static string ToCsharpIdent(this string file, TemplateParameterType type)
        {
            switch (type)
            {
                case TemplateParameterType.File:
                    return Path.GetFileNameWithoutExtension(file).ToCsharpIdent(false);
                case TemplateParameterType.Directory:
                    return Path.GetDirectoryName(file).ToCsharpIdent(false);
                case TemplateParameterType.Extension:
                    return Path.GetFullPath(file).Replace(Path.GetExtension(file), "").ToCsharpIdent(false);
                case TemplateParameterType.None:
                default:
                    return file.ToCsharpIdent(false);
            }
        }

        public static string ToCsharpIdent(this string name, bool pascalCase = true)
        {
            var sb = new StringBuilder();

            if (!char.IsLetter(name[0]))
            {
                sb.Append('_').AppendIdent(name[0]);
            }
            else if (pascalCase)
            {
                sb.Append(char.ToUpper(name[0]));
            }
            else
            {
                sb.Append(name[0]);
            }

            for (var i = 1; i < name.Length; i++)
            {
                sb.AppendIdent(name[i]);
            }

            var dashed = Regex.Replace(sb.ToString(), "_+", "_");
            var pascal = Regex.Replace(dashed, "_([a-zA-Z])", m => m.Groups[1].Value?.ToUpper() ?? string.Empty);
            return Regex.Replace(pascal, "[A-Z]{2,}", m => m.Value.ToTitle());
        }

        public static string ToCsharpInterface(this string ident) => $"IHave{ident}";

        private static void AppendIdent(this StringBuilder sb, char c)
        {
            if (char.IsLetter(c) || char.IsNumber(c) || c == '_')
            {
                sb.Append(c);
            }
            else if (c == '.' || c == '-' || c == ':' || c == ',' || char.IsWhiteSpace(c) || c == '/' || c == '\\')
            {
                sb.Append('_');
            }
        }
    }
}
