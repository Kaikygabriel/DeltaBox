using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class RemoveVersionCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args[1] is null)
            return Error.CommandInvalid();
        return RemoveVersion(ctx.Folder,ctx.Args[1]);
    }
    
    public Result RemoveVersion(string pathFromFolder, string nameVersion)
    {
        if (!Directory.Exists(pathFromFolder))
            return Error.DirectoryNotFound() ;
        
        var files = Directory.GetFiles(pathFromFolder);
        var deltaBoxFile = files.FirstOrDefault(x => Path.GetFileName(x).Equals(Configure.DeltaBoxFile)) ;
        
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
            if(parts.Length >=2)
                if ( parts[0]!="BranchCurrent"&&parts[0]!="CurrentVersion"&&parts[0]!="Branch")
                {
                    versionFinish = parts[1];
                }
        }
        var lines = File.ReadLines(deltaBoxFile).ToList();
        File.SetAttributes(deltaBoxFile, FileAttributes.Normal);

        lines[0] = $"CurrentVersion|{versionFinish}";
        File.WriteAllLines(deltaBoxFile, lines);
    }

   
}