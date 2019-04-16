namespace Simplet.Generator
{
    using System.Collections.Generic;
    using Simplet.Options;

    internal class CsProjGenerator : IGenerator
    {
        public IEnumerable<IGeneratedFile> Generate(SimpletOptions options) => new IGeneratedFile[]
        {
            new TextFile(
                $"{options.ProjectName}.csproj",
                $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>{options.TargetFramework}</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageId>{options.ProjectName}</PackageId>
    <Product>{options.ProjectName}</Product>
    <PackageOutputPath>./</PackageOutputPath>
  </PropertyGroup>

</Project>
"
            )
        };
    }
}
