namespace Simplet.Generator
{
    using System;
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

        private static string GetVersion(string version)
        {
            switch (version)
            {
                case "":
                case "auto":
                case null:
                    return GetAutoVersion();
                default:
                    return version;
            }
        }

        private static string GetAutoVersion() => $"1.0.0-pre.{DateTime.Now.ToString("yyyy.M.d")}.{Environment.GetEnvironmentVariable("Build.BuildId") ?? "0"}";
    }
}
