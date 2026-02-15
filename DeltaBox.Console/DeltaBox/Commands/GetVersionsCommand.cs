using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class GetVersionsCommand : ICommand
{
    public Result Get(string pathFromFolder)
    {
        Console.WriteLine(" \nVersions :\n");

        if (!Directory.Exists(pathFromFolder))
            return Error.DirectoryNotFound() ;
        
        var files = Directory.GetFiles(pathFromFolder);
        var deltaBoxFile = files.FirstOrDefault(x => Path.GetFileName(x).Equals("deltabox"));
        
        if (deltaBoxFile is null)
            return Error.DeltaBoxNotFound();
        var branchCurrent = "";

        var deltabox = File.ReadLines(deltaBoxFile);
        foreach (var line in deltabox)
        {
            var parts = line.Split('|');

            if (parts[0] == "BranchCurrent")
                branchCurrent = parts[1];
        }
        foreach (var line in deltabox)
        {
            var parts = line.Split('|');
            if (parts.Length == 3 && parts[0] != "BranchCurrent"&& parts[0].Equals(branchCurrent))
            {
                Console.WriteLine($"\t{parts[0]} - {parts[1]}");
            }
        }
        Console.WriteLine($"BRANCH : {branchCurrent}");
        return Result.Success();
    }

    public Result Execute(CommandContext ctx)
    {
        return Get(ctx.Folder);
    }
}