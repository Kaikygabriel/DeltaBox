namespace DeltaBox.Abstraction;

public interface IFileSystem
{
    void WriteAll(string path, IEnumerable<string> content);
    void Append(string path, params List<string> content);
    void Write(string path, string content);
    void WriteBytes(string path, byte[] bytes);
}