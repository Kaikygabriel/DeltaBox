using System.Security.Cryptography;
using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Services;

public class AltersFilesHandler : ICommand
{
    
    public Result Execute(CommandContext ctx)
    {
        return ViewAltersFiles(ctx.Folder);
    }
    public Result ViewAltersFiles(string pathFromFolder)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return Error.DeltaBoxNotFound();
        var result = new Dictionary<string, string>();

        var versionFinish = "";
        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 2 && parts[0] != "CurrentVersion")
            {
                versionFinish = parts.First();
            }
        }

        Console.WriteLine(versionFinish);
        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length >= 3 && parts.Contains(versionFinish) )
            {
                 result[parts[parts.Length-2]] = parts[parts.Length-1];
            }
        }
    
        Console.WriteLine("\n     Files Changes :\n");
        GetModifiedFiles(files, result);
        ActiveGetModifiedInSubDictionary(Directory.GetDirectories(pathFromFolder), result);
        Console.WriteLine("\n     New Files  :\n");
        GetNewFiles(files, result);
        ActiveGetNewInSubDictionary(Directory.GetDirectories(pathFromFolder), result);

        return Result.Success();
    }
    private void ActiveGetNewInSubDictionary(IEnumerable<string> dictionaries,Dictionary<string, string> result)
    {
        foreach (var d in dictionaries)
        {
            GetNewFiles(Directory.GetFiles(d),result);   
            var subSubDictionaries = Directory.GetDirectories(d);
            if (subSubDictionaries.Length >= 1)
                ActiveGetNewInSubDictionary(subSubDictionaries,result);
        }
    }
    private void ActiveGetModifiedInSubDictionary(IEnumerable<string> dictionaries,Dictionary<string, string> result)
    {
        foreach (var d in dictionaries)
        {
            GetModifiedFiles(Directory.GetFiles(d),result);   
            var subSubDictionaries = Directory.GetDirectories(d);
            if (subSubDictionaries.Length >= 1)
                ActiveGetModifiedInSubDictionary(subSubDictionaries,result);
        }
    }
    private void GetModifiedFiles(IEnumerable<string> files,Dictionary<string, string> result)
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
                    Console.WriteLine("\tModified :    "+filePath);
                    Console.ForegroundColor = currentTextColor;
                }
            }
        }
    }
    private void GetNewFiles(IEnumerable<string> files,Dictionary<string, string> result)
    {
        foreach (var filePath in files)
        {
            byte[] currentContent = File.ReadAllBytes(filePath);

            var key = result.Keys.FirstOrDefault(x => x.Equals(filePath));
            string fileName = Path.GetFileName(filePath);
            
            if(key is null && fileName != "deltabox")
            {
                var currentTextColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\tNew File :    "+filePath);
                Console.ForegroundColor = currentTextColor;
            }
        }
    }
}