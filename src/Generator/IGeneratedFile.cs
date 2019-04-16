namespace Simplet.Generator
{
    public interface IGeneratedFile
    {
        string Path { get; }

        byte[] Content { get; }
    }
}
