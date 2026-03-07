using DeltaBox.Abstraction;
using DeltaBox.Commum;
using DeltaBox.Service;
using DeltaBox.Storage;

namespace DeltaBox.Commands;

public class InitVersionCommand : ICommand
{
    private readonly GetFilesIgnore _filesIgnore;
    private readonly IDeltaBoxStorage _storage;

    public InitVersionCommand()
    {
        _filesIgnore = new();
        _storage = new DeltaBoxStorage(new FileSystem());
    }

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


        _storage.ChangeCurrentVersion(fileDeltaBox,"Init");
        //File.AppendAllText(fileDeltaBox, $"CurrentVersion|Init\n");
        
        _storage.ChangeCurrentBranch(fileDeltaBox,"main");
        //File.AppendAllText(fileDeltaBox, $"\nBranchCurrent|main\n");

        _storage.AddNewBranch(fileDeltaBox, " ", "Init", "main");
        //File.AppendAllText(fileDeltaBox, $"\nBranch|main||Init\n");

        _storage.AddNewVersion(fileDeltaBox, "main", "Init");
        //File.AppendAllText(fileDeltaBox, $"\nmain|Init|{DateTime.UtcNow}\n");

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
        var fileDeltaBoxIgnore = Path.Combine(pathFromFolder,Configure.DeltaBoxIgnoreFile);
        for (var a = 0; a < files.Length; a++)
        {
            if (_filesIgnore.FilesIgnore(files[a].Split(new[]{'/','\\'}).Last(),fileDeltaBoxIgnore))
            {
                
            }
            else
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
}