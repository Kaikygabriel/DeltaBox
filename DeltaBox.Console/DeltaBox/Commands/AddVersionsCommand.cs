using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class AddVersionsCommand : ICommand
{
    
    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args[1] is null)
            return new Error("Command.Invalid", "INVALID COMMAND");

        return AddNewVersion(ctx.Folder, ctx.Args[1]);
    }
    
    private Result AddNewVersion(string pathFromFolder,string nameVersion)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return Error.DeltaBoxNotFound();
        var result = new Dictionary<string, string>();
        var currentBranch = "";
        
        foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 3 && parts[1] == nameVersion)
                result[parts[1]] = parts[2];
            if (parts[0] == "BranchCurrent")
            {
                currentBranch = parts[1];
            }
        }

        if (string.IsNullOrEmpty(currentBranch))
            return Result.Failure(new("Branch.NotFound", "NOt Found")); 
        
        File.AppendAllText(pathFromFolder + "/deltabox", $"\n{currentBranch}|{nameVersion}|{DateTime.UtcNow}\n");

        UpdateFilesInDeltaBox(files, result, nameVersion, pathFromFolder,currentBranch);

        
        var dictionaries = Directory.GetDirectories(
            pathFromFolder,
            "*",
            SearchOption.AllDirectories);

        foreach (var subDictionary in dictionaries)
        {
            UpdateFilesInDeltaBox(Directory.GetFiles(subDictionary), result, nameVersion, pathFromFolder,currentBranch);
        }  
        
        var lines = File.ReadLines(pathFromFolder + "/deltabox").ToList();
        
        lines[0] = $"CurrentVersion|{nameVersion}";

        File.WriteAllLines(pathFromFolder + "/deltabox", lines);

        return Result.Success();
    }

    private void UpdateFilesInDeltaBox(string[] files,Dictionary<string, string>result,string nameVersion,string pathFromFolder,string currentBranch)
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
                    var text = $"{currentBranch}|{nameVersion}|{filePath}|{Convert.ToBase64String(currentContent)}\n";
                    if (OperatingSystem.IsLinux()|| OperatingSystem.IsMacOS())
                        text = $"{currentBranch}|{nameVersion}|{filePath.Replace('\\','/')}|{Convert.ToBase64String(currentContent)}\n";

                    Console.WriteLine($"Versionando : {text} ");
                    File.AppendAllText(pathFromFolder + "/deltabox", text);
                }
            }
            else if (fileName != "deltabox")
            {
                byte[] content = File.ReadAllBytes(filePath);
                var fileInBase64 = Convert.ToBase64String(content);
                var text = $"{currentBranch}|{nameVersion}|{filePath}|{fileInBase64}\n";
                if (OperatingSystem.IsLinux()|| OperatingSystem.IsMacOS())
                    text = $"{currentBranch}|{nameVersion}|{filePath.Replace('\\','/')}|{fileInBase64}\n";
                
                Console.WriteLine($"Versionando : {text} ");
                File.AppendAllText(pathFromFolder + "/deltabox", text);
            }
        }
        
    }
}