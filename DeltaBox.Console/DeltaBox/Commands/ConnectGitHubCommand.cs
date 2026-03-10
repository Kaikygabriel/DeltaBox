using DeltaBox.Abstraction;
using DeltaBox.Commum;
using LibGit2Sharp;

namespace DeltaBox.Commands;

internal sealed class ConnectGitHubCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        using var repo = new Repository(ctx.Folder);

        var remote = repo.Network.Remotes["origin"];

        Console.Write("Url do repositorio remoto: ");
        var url = Console.ReadLine();
        Console.Write("Seu Nome GitHub: ");
        var name = Console.ReadLine();
        Console.Write("Seu Token GitHub: ");
        var token = ReadSecret();
    
        if (remote == null)
        {
            repo.Network.Remotes.Add("origin", url);
            Console.WriteLine("Remote origin add.");
        }

        var fetchOptions = new FetchOptions
        {
            CredentialsProvider = (_url, _user, _cred) =>
                new UsernamePasswordCredentials
                {
                    Username = name,
                    Password = token
                }
        };

        var remoteRepo = repo.Network.Remotes["origin"];

        LibGit2Sharp.Commands.Fetch(repo, remoteRepo.Name, remoteRepo.FetchRefSpecs.Select(x => x.Specification), fetchOptions,
            null);

        Console.WriteLine("Conectado ao repositório remoto com sucesso!");
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