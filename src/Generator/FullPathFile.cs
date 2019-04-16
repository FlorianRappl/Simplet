namespace Simplet.Generator
{
    using System.IO;

    internal class FullPathFile : IGeneratedFile
    {
        public FullPathFile(DirectoryInfo rootDir, IGeneratedFile file)
        {
            Path = System.IO.Path.Combine(rootDir.FullName, file.Path);
            Content = file.Content;
        }

        public string Path { get; }

        public byte[] Content { get; }
    }
}
