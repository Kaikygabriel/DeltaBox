using DeltaBox.Abstraction;
using DeltaBox.Commum;
using LibGit2Sharp;

namespace DeltaBox.Commands;

internal sealed class CloneCommand : ICommand
{
    private readonly NewOsCommand _command;

    public CloneCommand()
    {
        _command = new();
    }

    public Result Execute(CommandContext ctx)
    {
        Console.Write("Repository remote : ");
        string repoUrl = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(repoUrl))
            return new Error("Repository.Invalid", "Repository is null or empty");
        
        string path = ctx.Folder;

        Repository.Clone(repoUrl, path);
        _command.Execute(ctx);
        return Result.Success();
    }
}