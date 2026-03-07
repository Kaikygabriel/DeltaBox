namespace DeltaBox.Abstraction;

public interface IFileSystem
{
    void WriteAll(string path, IEnumerable<string> content);
    void AppendLines(string path, List<string> content);
    void AppendText(string path, string content);

    void Write(string path, string content);
    void WriteBytes(string path, byte[] bytes);
    
    string[] ReadAllLines(string path);
}