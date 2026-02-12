using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class RemoveVersionCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args[2] is null)
            return Error.CommandInvalid();
        return RemoveVersion(ctx.Folder,ctx.Args[2]);
    }
    
    public Result RemoveVersion(string pathFromFolder, string nameVersion)
    {
        if (!Directory.Exists(pathFromFolder))
            return Error.DirectoryNotFound() ;
        
        var files = Directory.GetFiles(pathFromFolder);
        var deltaBoxFile = files.FirstOrDefault(x => Path.GetFileName(x).Equals("deltabox")) ?? throw new Exception("NÂO ACHOU O ARQUIVO ");
        
        if (deltaBoxFile is null)
            return Error.DeltaBoxNotFound();
        
        var linesInDelta = File.ReadLines(deltaBoxFile).ToList();
        foreach (var line in File.ReadLines(deltaBoxFile))
        {
            var parts = line.Split('|');
            if (parts.Length >= 2)
            {
                if (parts[1].Equals(nameVersion, StringComparison.CurrentCultureIgnoreCase))
                    linesInDelta.Remove(line);      
            }
        }
        File.WriteAllLines(deltaBoxFile, linesInDelta);

        var versionFinish = "";
        foreach (var line in File.ReadLines(deltaBoxFile))
        {
            var parts = line.Split('|');
            if (parts.Length == 2&& parts[0]!="BranchCurrent")
            {
                versionFinish = parts.First();
            }
        }
        UpdateCurrentVersion(deltaBoxFile);
        
        return Result.Success();
    }

    private void UpdateCurrentVersion(string deltaBoxFile)
    {
        var versionFinish = "";
        foreach (var line in File.ReadLines(deltaBoxFile))
        {
            var parts = line.Split('|');
            if (parts.Length == 2 && parts[0]!="BranchCurrent")
            {
                versionFinish = parts.First();
            }
        }
        var lines = File.ReadLines(deltaBoxFile).ToList();
        lines[0] = $"CurrentVersion|{versionFinish}";
        File.WriteAllLines(deltaBoxFile, lines);
    }

   
}