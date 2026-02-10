using System.Text;

namespace DeltaBox.Services;

public class AddVersionsHandler
{
    public bool Create(string pathOfFiles,string nameVersion)
    {
        AddNewVersion(pathOfFiles,nameVersion);
        return true;
    }

    private bool AddNewVersion(string pathFromFolder,string nameVersion)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return false;
        var result = new Dictionary<string, string>();

        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 3 && parts[0] == nameVersion)
                result[parts[1]] = parts[2];
        }

        File.AppendAllText(pathFromFolder + "/deltabox", $"\n{nameVersion}|{DateTime.UtcNow}\n");

        UpdateFilesInDeltaBox(files, result, nameVersion, pathFromFolder);

        
        var dictionaries = Directory.GetDirectories(
            pathFromFolder,
            "*",
            SearchOption.AllDirectories);

        foreach (var subDictionary in dictionaries)
        {
            UpdateFilesInDeltaBox(Directory.GetFiles(subDictionary), result, nameVersion, pathFromFolder);
        }  
        
        return true;
    }

    private void UpdateFilesInDeltaBox(string[] files,Dictionary<string, string>result,string nameVersion,string pathFromFolder)
    {
         
        foreach (var filePath in files)
        {
            byte[] currentContent = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            var key = result.Keys.FirstOrDefault(x => x.Equals(filePath, StringComparison.CurrentCultureIgnoreCase));
            if (key is not null && fileName != "deltabox")
            {
                var fileInBytePrevius = Convert.FromBase64String(result.GetValueOrDefault(key) ?? string.Empty);

                if (!currentContent.SequenceEqual(fileInBytePrevius))
                {
                    var text = $"{nameVersion}|{filePath}|{Convert.ToBase64String(currentContent)}\n";
                    Console.WriteLine($"Versionando : {text} ");
                    File.AppendAllText(pathFromFolder + "/deltabox", text);
                }
            }
            else if (fileName != "deltabox")
            {
                byte[] content = File.ReadAllBytes(filePath);
                var fileInBase64 = Convert.ToBase64String(content);
                var text = $"{nameVersion}|{filePath}|{fileInBase64}\n";
                if (OperatingSystem.IsLinux()|| OperatingSystem.IsMacOS())
                    text = $"{nameVersion}|{filePath.Replace('\\','/')}|{fileInBase64}\n";
                
                Console.WriteLine($"Versionando : {text} ");
                File.AppendAllText(pathFromFolder + "/deltabox", text);
            }
        }

    }
}