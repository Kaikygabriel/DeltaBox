using DeltaBox.Abstraction;
using DeltaBox.Commum;
using LibGit2Sharp;

namespace DeltaBox.Commands;

internal sealed class PullCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        var repoPath = ctx.Folder;

        using var repo = new Repository(repoPath);

        if (!repo.Network.Remotes.Any())
        {
            return new Error("Repository.NotRemote","Not Found remote repository this folder !");
        }
        
        Console.Write("Seu Nome GitHub: ");
        var name = Console.ReadLine();
        Console.Write("Seu Token GitHub: ");
        var token = ReadSecret();
        
        var signature = new Signature(
            name,
            $"{name}@users.noreply.github.com",
            DateTimeOffset.Now
        );

        var options = new PullOptions()
        {
            FetchOptions = new FetchOptions()
            {
                CredentialsProvider = (_url, _user, _cred) =>
                    new UsernamePasswordCredentials
                    {
                        Username = name,
                        Password = token
                    }
            }
        };

        var result = LibGit2Sharp.Commands.Pull(repo, signature, options);

        Console.WriteLine($"Status of merge: {result.Status}");
        return Result.Success();
    }
    private static string ReadSecret()
    {
        var password = "";

        ConsoleKeyInfo key;

        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            password += key.KeyChar;
        }
        return password;
    }
}