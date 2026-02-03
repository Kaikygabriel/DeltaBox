using System.Text;

namespace DeltaBox.Services;

public class InitVersionHandler
{
    public bool Create(string pathOfFiles)
    {
        TryInitVersionInFolder(pathOfFiles);
        return true;
    }

    private bool TryInitVersionInFolder(string pathFromFolder)
    {
        try
        {
            if (!Directory.Exists(pathFromFolder))
                return false;
            var files = Directory.GetFiles(pathFromFolder);
            
            for(var a = 0; a< files.Length ; a++)
            {
                byte[] content = File.ReadAllBytes(files[a]);
                var fileInBase64 = Convert.ToBase64String(content);
                var text = $"Init|{Path.GetFileName(files[a])}|{fileInBase64}\n";
                Console.WriteLine($"Versionando :  {text}");
                File.AppendAllText(pathFromFolder + "/deltabox", text);
            }
            
            var result = new Dictionary<string, string>();
            
                foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
                {
                    var parts = line.Split('|'); // Formato: NomeDoArquivo|Hash
                    if (parts.Length == 3) result[parts[1]] = parts[2];
                }
            
            
            //foreach(var r in result)
            //    Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(r.Value)));
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}