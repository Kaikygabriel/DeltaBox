using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class InitVersionCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        return InitVersionInFolder(ctx.Folder);
    }

    private Result InitVersionInFolder(string pathFromFolder)
    {
        if (!Directory.Exists(pathFromFolder))
            return Error.DirectoryNotFound();
        var files = Directory.GetFiles(pathFromFolder);

        if (files.Any(x => Path.GetFileName(x).Equals(Configure.DeltaBoxFile)))
            return new Error("DeltaBox.AlreadyExists", "File deltabox already exists!");

        var fileDeltaBox = Path.Combine(pathFromFolder,Configure.DeltaBoxFile);

        File.AppendAllText(fileDeltaBox, $"CurrentVersion|Init\n");
        
        File.AppendAllText(fileDeltaBox, $"\nBranchCurrent|main\n");
        File.AppendAllText(fileDeltaBox, $"\nBranch|main||Init\n");
        
        File.AppendAllText(fileDeltaBox, $"\nmain|Init|{DateTime.UtcNow}\n");

        SaveFilesOfDirectory(files, pathFromFolder);

        var dictionaries = Directory.GetDirectories(
            pathFromFolder,
            "*",
            SearchOption.AllDirectories);

        foreach (var subDictionary in dictionaries)
        {
            var filesInSubDictionary = Directory.GetFiles(subDictionary);
            SaveFilesOfDirectory(filesInSubDictionary, pathFromFolder);
        }
        if(OperatingSystem.IsWindows())
            File.SetAttributes(Path.Combine(pathFromFolder ,Configure.DeltaBoxFile), FileAttributes.Hidden);

        return Result.Success();
    }

    private void SaveFilesOfDirectory(string[] files,string pathFromFolder)
    {
        var fileDeltaBox = Path.Combine(pathFromFolder,Configure.DeltaBoxFile);
        for (var a = 0; a < files.Length; a++)
        {
            byte[] content = File.ReadAllBytes(files[a]);
            var fileInBase64 = Convert.ToBase64String(content);
            var text = $"main|Init|{files[a]}|{fileInBase64}\n";
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                text = $"main|Init|{files[a].Replace('\\','/')}|{fileInBase64}\n";
            Console.WriteLine($"Versionando :  {text}");
            File.AppendAllText(fileDeltaBox, text);
        }
    }
}