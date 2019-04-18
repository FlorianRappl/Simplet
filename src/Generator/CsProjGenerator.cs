namespace Simplet.Generator
{
    using System.Collections.Generic;
    using Simplet.Options;
    using static Simplet.Utils.VersionUtils;

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
    <PackageRequireLicenseAcceptance>false</PackageRequireLicenseAcceptance>
    <Description>{options.Description}</Description>
    <PackageId>{options.ProjectName}</PackageId>
    <Product>{options.ProjectName}</Product>
    <Authors>{options.Author}</Authors>
    <Version>{GetVersion(options.Version)}</Version>
    <DocumentationFile>{options.ProjectName}.xml</DocumentationFile>
    <noWarn>1591,NU5105</noWarn>
    <PackageOutputPath>./</PackageOutputPath>
  </PropertyGroup>

</Project>
"
            )
        };

    }
}
