using DeltaBox.Abstraction;
using DeltaBox.Commum;
using LibGit2Sharp;

namespace DeltaBox.Commands;

public class PushCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        var deltaboxPath = Path.Combine(ctx.Folder, Configure.DeltaBoxFile); 
        if (!File.Exists(deltaboxPath))
            return Error.DeltaBoxNotFound();
        
        if (!Repository.IsValid(ctx.Folder))
        {
            Repository.Init(ctx.Folder);
        }
        using var repo = new Repository(ctx.Folder);
        
        var commitsInDeltaBox = File.ReadAllLines(deltaboxPath);

        if (commitsInDeltaBox.Length == 0)
            return new Error("Commits.NotFound", "Commits not found or is null");
        
        Console.Write("Seu Nome GitHub: ");
        var name = Console.ReadLine();
        Console.Write("Seu Token GitHub: ");
        var token = ReadSecret();
        
        if (repo.Commits.Any(x => commitsInDeltaBox.Last().Equals(x.Message)))
        {
        }
        else
        {
            if (repo.RetrieveStatus().IsDirty)
            {
                LibGit2Sharp.Commands.Stage(repo, "*");
                LibGit2Sharp.Commands.Unstage(repo, ".deltabox");
                
                var author = new Signature(
                    name,
                    $"{name}@users.noreply.github.com",
                    DateTimeOffset.Now
                );

                repo.Commit( commitsInDeltaBox.Last(),author, author);
            }
        }
        
        var remote = repo.Network.Remotes["origin"];

        if (remote == null)
        {
            try
            {
                Console.Write("Url do repositório GitHub: ");
                var url = Console.ReadLine();

                remote = repo.Network.Remotes.Add("origin", url);
            }
            catch (Exception e)
            {
                return new Error("Url.Invalid", "url passed invalid");
            }
        }
        
        var options = new PushOptions
        {
            CredentialsProvider = (url, usernameFromUrl, types) =>
                new UsernamePasswordCredentials
                {
                    Username = name,
                    Password = token
                }
        };
        var pushRefSpec = $"refs/heads/{repo.Head.FriendlyName}";
        
        var branch = repo.Head;

        if (branch.TrackedBranch == null)
        {
            repo.Branches.Update(branch, b =>
            {
                b.Remote = "origin";
                b.UpstreamBranch = branch.CanonicalName;
            });
        }

        LibGit2Sharp.Commands.Pull(repo,
            new Signature(
                name,
                $"{name}@users.noreply.github.com",
                DateTimeOffset.Now
            ),
            new PullOptions());

        try
        {
            repo.Network.Push(remote, pushRefSpec, options);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
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