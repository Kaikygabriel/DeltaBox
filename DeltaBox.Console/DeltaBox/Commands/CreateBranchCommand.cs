using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class CreateBranchCommand: ICommand
{
    public Result Create(string folder, string nameNewBranch)
    {
        var files = Directory.GetFiles(folder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return Error.DeltaBoxNotFound();
        
        var fileDeltaBox = folder + "/deltabox";
        var branchCurrent = "";
        var lines = File.ReadLines(fileDeltaBox);
        var currentVersion = "";
        
        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts[0] == "BranchCurrent")
            {
                branchCurrent = parts[1];   
                if (parts[1].Equals(nameNewBranch))
                    return new Error("Branch.Already", "Branch Already exists !");
            }
            
            if (parts[0] == "CurrentVersion")
            {
                currentVersion = parts[1];
            }
        }
        var linesInDelta = File.ReadLines(fileDeltaBox).ToList();
        linesInDelta[2] = $"BranchCurrent|{nameNewBranch}";
        linesInDelta.Add($"Branch|{nameNewBranch}|{branchCurrent}|{currentVersion}");
        File.WriteAllLines(fileDeltaBox, linesInDelta);
        return Result.Success();
    }

    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args.Length <= 1|| ctx.Args[2] is null)
            return Error.CommandInvalid();
        return Create(ctx.Folder, ctx.Args[2]);
    }
}