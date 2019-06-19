using System.Collections.Generic;

namespace Simplet.Generator
{
    internal class TextClass : TextFile, IGeneratedClass
    {
        public TextClass(string path, string content, IDictionary<string, string> resources) : base(path, content)
        {
            ResourceStrings = resources;
        }

        public IDictionary<string, string> ResourceStrings { get; }
    }
}
