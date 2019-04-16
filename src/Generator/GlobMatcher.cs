namespace Simplet.Generator
{
    using System.Collections.Generic;
    using Microsoft.Extensions.FileSystemGlobbing;

    internal class GlobMatcher
    {
        private readonly Matcher _matcher;

        public GlobMatcher(IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            var matcher = new Matcher();

            matcher.AddIncludePatterns(includes);
            matcher.AddExcludePatterns(excludes);

            _matcher = matcher;
        }

        public bool IsIncluded(string file)
        {
            return _matcher.Match(file).HasMatches;
        }
    }
}
