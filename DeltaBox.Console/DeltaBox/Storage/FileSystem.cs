using DeltaBox.Abstraction;

namespace DeltaBox.Storage;

public sealed class FileSystem : IFileSystem
{
    public void WriteAll(string path, IEnumerable<string> content)
    {
        File.WriteAllLines(path, content);
    }

    public void AppendText(string path, string content)
    {
        File.AppendAllText(path,content);
    }
    public void AppendLines(string path, List<string> content)
    {
        File.AppendAllLines(path,content);
    }

    public void Write(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public void WriteBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }
    public string[] ReadAllLines(string path)
        => File.ReadAllLines(path);
}