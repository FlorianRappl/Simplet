namespace Simplet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Simplet.Generator;
    using Simplet.Options;

    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                Console.WriteLine("No argument provided. Require a config file.");
                Environment.Exit(1);
            }
            else if (args.Length > 1)
            {
                PrintUsage();
                Console.WriteLine("More than one argument seen. Expected only a single config file.");
                Environment.Exit(1);
            }
            else if (!File.Exists(args[0]))
            {
                PrintUsage();
                Console.WriteLine($"The provided config file '{args[0]}' does not exist.");
                Environment.Exit(1);
            }
            else
            {
                var files = GenerateFiles(args[0]);

                foreach (var file in files)
                {
                    Console.Write($"Writing '{file.Path}' ... ");
                    File.WriteAllBytes(file.Path, file.Content);
                    Console.WriteLine("done!");
                }
            }
        }

        private static IEnumerable<IGeneratedFile> GenerateFiles(string configPath)
        {
            var options = SimpletOptions.ReadFrom(configPath);
            var dirInfo = Directory.CreateDirectory(options.TargetDirectory);
            var csGenerator = new CsGenerator();
            var csFiles = csGenerator.Generate(options).ToArray();
            var txtGenerator = new TxtGenerator(csFiles.OfType<IGeneratedClass>());
            var txtFiles = txtGenerator.Generate(options);
            var csprojGenerator = new CsProjGenerator();
            var csprojFiles = csprojGenerator.Generate(options);

            foreach (var file in csprojFiles.Concat(txtFiles.Concat(csFiles)))
            {
                yield return new FullPathFile(dirInfo, file);
            }
        }

        private static void PrintUsage()
        {
            var versionString = Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion
                .ToString();

            Console.WriteLine($"Simplet v{versionString}");
            Console.WriteLine("---------------------");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  simplet <config-file>");
            Console.WriteLine();
        }
    }
}
