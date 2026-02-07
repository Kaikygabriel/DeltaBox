using System.Security.Cryptography;

namespace DeltaBox.Services;

public static class AltersFilesHandler
{
    
    public static void ViewAltersFiles(string pathFromFolder)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return ;
        var result = new Dictionary<string, string>();

        var currentVersion = "";
        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 1)
            {
                currentVersion = parts.Last();
            }
        }
        
        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length >= 3 && parts.Contains(currentVersion) )
            {
                 result[parts[parts.Length-2]] = parts[parts.Length-1];
            }
        }
    
        Console.WriteLine("     Files Changes :\n");
        GetAltersInFile(files, result);
        ActiveInSubDictionary(Directory.GetDirectories(pathFromFolder), result);

    }

    private static void ActiveInSubDictionary(IEnumerable<string> dictionaries,Dictionary<string, string> result)
    {
        foreach (var d in dictionaries)
        {
            GetAltersInFile(Directory.GetFiles(d),result);   
            var subSubDictionaries = Directory.GetDirectories(d);
            if (subSubDictionaries.Length >= 1)
                ActiveInSubDictionary(subSubDictionaries,result);
        }
    }
    private static void GetAltersInFile(IEnumerable<string> files,Dictionary<string, string> result)
    {
        foreach (var filePath in files)
        {
            byte[] currentContent = File.ReadAllBytes(filePath);

            var key = result.Keys.FirstOrDefault(x => x.Equals(filePath));
            string fileName = Path.GetFileName(filePath);
            if (key is not null)
            {
                var fileInBytePrevius = Convert.FromBase64String(result[key]);
                if (!currentContent.SequenceEqual(fileInBytePrevius))
                {
                    var currentTextColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(" Modified :    "+filePath);
                    Console.ForegroundColor = currentTextColor;
                }
            }
            if(key is null && fileName != "deltabox")
            {
                var currentTextColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" New File :    "+filePath);
                Console.ForegroundColor = currentTextColor;
            }
        }
    }
}