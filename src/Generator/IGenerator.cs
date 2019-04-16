namespace Simplet.Generator
{
    using System.Collections.Generic;
    using Simplet.Options;

    public interface IGenerator
    {
        IEnumerable<IGeneratedFile> Generate(SimpletOptions options);
    }
}
