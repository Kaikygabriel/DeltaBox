using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class CheckoutCommand : ICommand
{
    public Result Checkout(string folder,string branchName)
    {
        var files = Directory.GetFiles(folder);
        if (!files.Any(x => Path.GetFileName(x) == Configure.DeltaBoxFile))
            return Error.DeltaBoxNotFound();
        
        var fileDeltaBox = folder + "/"+Configure.DeltaBoxFile;
        var lines = File.ReadLines(fileDeltaBox);
        bool existsBranch = false; 
        
        foreach (var line in lines)
        {
            var parts = line.Split('|');
           
            if (parts[0] == "Branch")
            {
                if (parts[1].Equals(branchName))
                    existsBranch = true;
            }
        }

        if (!existsBranch)
            return new Error("Branch.NotFound", "Branch not found 1");
        
        var linesInDelta = File.ReadLines(fileDeltaBox).ToList();
        linesInDelta[2] = $"BranchCurrent|{branchName}";
        
        var versionFinish = "";
        
        foreach (var line in linesInDelta)
        {
            var parts = line.Split('|');
            if (parts.Length >= 4 && parts[0] == branchName)
            {
                versionFinish = parts[1];
            }
        }
        
        linesInDelta[0] = $"CurrentVersion|{versionFinish}";
        
        File.WriteAllLines(fileDeltaBox, linesInDelta);

        var versionCommand = new VersionsCommand();
        
        versionCommand.GoToVersion(folder, versionFinish);
        
        return Result.Success();
    }

    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args.Length <= 1|| ctx.Args[1] is null)
            return Error.CommandInvalid();
        return Checkout(ctx.Folder,ctx.Args[1]);
    }
}