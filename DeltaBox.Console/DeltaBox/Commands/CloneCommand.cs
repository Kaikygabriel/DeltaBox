using DeltaBox.Abstraction;
using DeltaBox.Commum;
using LibGit2Sharp;

namespace DeltaBox.Commands;

internal sealed class CloneCommand : ICommand
{

    public CloneCommand()
    {
    }

    public Result Execute(CommandContext ctx)
    {
        string repoUrl = ctx.Args[1];
        if (string.IsNullOrWhiteSpace(repoUrl))
            return new Error("Repository.Invalid", "Repository is null or empty");
        
        string path = ctx.Folder;
        var repoName = repoUrl.Split('/').Last().Replace(".git", "");

        Repository.Clone(repoUrl, repoName);
        return Result.Success();
    }
}