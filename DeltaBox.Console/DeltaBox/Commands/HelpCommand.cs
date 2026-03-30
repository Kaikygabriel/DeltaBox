using System.Text;
using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;


public sealed class HelpCommand : ICommand
{
    private readonly Dictionary<string, CommandHelp> _help;

    public HelpCommand()
    {
        _help = new(StringComparer.OrdinalIgnoreCase)
        {
            ["init"] = new CommandHelp(
                "init",
                "Initialize the DeltaBox repository (required to begin).",
                "deltabox init",
                examples: new[]
                {
                    "deltabox init"
                }
            ),

            ["prev"] = new CommandHelp(
                "prev <version>",
                "Revert the project to a previous version.",
                "deltabox prev <versao>",
                examples: new[]
                {
                    "deltabox prev init",
                    "deltabox prev v2"
                }
            ),

            ["status"] = new CommandHelp(
                "status",
                "Shows changes to files since the last commit.",
                "deltabox status",
                examples: new[]
                {
                    "deltabox status"
                }
            ),

            ["commit"] = new CommandHelp(
                "commit <name>",
                "Create a new version (commit) with a name.",
                "deltabox commit <nome>",
                examples: new[]
                {
                    "deltabox commit \"Create a login screen.\" ",
                    "deltabox commit v1.0.0"
                }
            ),

            ["log"] = new CommandHelp(
                "log",
                "Lists commits and shows the current branch.",
                "deltabox log",
                examples: new[]
                {
                    "deltabox log"
                }
            ),

            ["remove"] = new CommandHelp(
                "remove <branch>",
                "Remove a branch.",
                "deltabox remove <branch>",
                examples: new[]
                {
                    "deltabox remove feature-login"
                }
            ),

            ["branch"] = new CommandHelp(
                "branch <name>",
                "Create a new branch.",
                "deltabox branch <name>",
                examples: new[]
                {
                    "deltabox branch feature-login"
                }
            ),

            ["checkout"] = new CommandHelp(
                "checkout <branch>",
                "Switch to another branch.",
                "deltabox checkout <branch>",
                examples: new[]
                {
                    "deltabox checkout main",
                    "deltabox checkout feature-login"
                }
            ),

            ["merge"] = new CommandHelp(
                "merge <branch>",
                "Merges the <branch> into the current branch (only when the merge destination is = main).",
                "deltabox merge <branch> <nameVersion>",
                rules: new[]
                {
                    "The merge can only be done towards main, for now.",
                    "Merge is only allowed when the parent branch is \"main\".",
                    "The merge brings the changes from the <branch> to the main branch."
                },
                examples: new[]
                {
                    "deltabox checkout feature-login",
                    "deltabox merge feature-login"
                }
            ),
            
            ["push"] = new CommandHelp(
                "push",
                "These are commits to a remote repository on GitHub.",
                "push",
                examples: new[]
                {
                    "deltabox push\n\tSeu Nome GitHub: name*\n\tSeu Token GitHub: githubToken*\n\tUrl do repositório GitHub: urlRemote*"
                }
            ),
            ["pull"] = new CommandHelp(
                "pull",
                "It brings changes from the remote repository to the local repository.",
                "pull",
                examples: new[]
                {
                    "deltabox pull\n\tSeu Nome GitHub: name*\n\tSeu Token GitHub: githubToken*"
                }
            ),
            ["connect"] = new CommandHelp(
                "connect",
                "Connect the repository local with remote.",
                "connect",
                examples: new[]
                {
                    "deltabox connect\n\tSua url: url*\n\tSeu Token GitHub: githubToken*"
                }
            )
        };
    }
    
    private void PrintGeneralHelp()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        Console.WriteLine(@"
        ╔══════════════════════════════╗
        ║        ██████╗ ██████╗       ║
        ║        ██╔══██╗██╔══██╗      ║
        ║        ██║  ██║██████╔╝      ║
        ║        ██║  ██║██╔══██╗      ║
        ║        ██████╔╝██████╔╝      ║
        ║        ╚═════╝ ╚═════╝       ║
        ║          D E L T A B O X     ║
        ╚══════════════════════════════╝
                                             ");
        
        Console.ResetColor();
        
        Console.WriteLine("DeltaBox — Simple versioning by folders");
        Console.WriteLine();
        Console.WriteLine("USE :");
        Console.WriteLine("  deltabox <command> [options]");
        Console.WriteLine();
        Console.WriteLine("COMMANDS:");

        var rows = _help
            .Select(kv => new { Name = kv.Key, Usage = kv.Value.Usage, Desc = kv.Value.ShortDescription })
            .OrderBy(x => x.Name)
            .ToList();

        var left = rows.Max(r => r.Usage.Length) + 2;

        foreach (var r in rows)
            Console.WriteLine($"  {r.Usage.PadRight(left)}{r.Desc}");

        Console.WriteLine();
        Console.WriteLine("TIP:");
        Console.WriteLine("  deltabox help <command>  Shows details and examples.");
    }

    private static void PrintCommandHelp(CommandHelp info)
    {
        Console.WriteLine("COMMAND:");
        Console.WriteLine($"  {info.Name}");
        Console.WriteLine();
        Console.WriteLine("DESCRIPTION:");
        Console.WriteLine($"  {info.ShortDescription}");

        if (info.Rules?.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine("RULES:");
            foreach (var rule in info.Rules)
                Console.WriteLine($"  • {rule}");
        }

        Console.WriteLine();
        Console.WriteLine("USE:");
        Console.WriteLine($"  {info.Syntax}");

        if (info.Examples?.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            foreach (var ex in info.Examples)
                Console.WriteLine($"  {ex}");
        }
    }
    

    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args.Length == 0)
        {
            PrintGeneralHelp();
            return Result.Success();
        }

        var cmd = "";

        if (ctx.Args is not null && ctx.Args.Length > 1)
            cmd = ctx.Args[1];
        if (!_help.TryGetValue(cmd, out var info))
        {
            if(!string.IsNullOrWhiteSpace(cmd))
                Console.WriteLine($"Command not found : {cmd}");
            PrintGeneralHelp();
            return Result.Success();
        }
        
        PrintCommandHelp(info);
        return Result.Success();

    }
}