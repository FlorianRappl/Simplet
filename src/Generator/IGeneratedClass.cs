using System.Collections.Generic;

namespace Simplet.Generator
{
    public interface IGeneratedClass : IGeneratedFile
    {
        IDictionary<string, string> ResourceStrings { get; }
    }
}
