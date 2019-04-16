namespace Simplet.Generator
{
    using System.Text;

    internal class TextFile : IGeneratedFile
    {
        private readonly string _path;
        private readonly byte[] _content;

        public TextFile(string path, string content)
        {
            _path = path;
            _content = Encoding.UTF8.GetBytes(content);
        }

        public string Path => _path;

        public byte[] Content => _content;
    }
}
