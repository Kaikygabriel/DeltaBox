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
                "Inicializa o repositório DeltaBox (obrigatório para começar).",
                "deltabox init",
                examples: new[]
                {
                    "deltabox init"
                }
            ),

            ["prev"] = new CommandHelp(
                "prev <versao>",
                "Volta o projeto para uma versão anterior.",
                "deltabox prev <versao>",
                examples: new[]
                {
                    "deltabox prev 3",
                    "deltabox prev v12"
                }
            ),

            ["status"] = new CommandHelp(
                "status",
                "Mostra alterações nos arquivos desde o último commit.",
                "deltabox status",
                examples: new[]
                {
                    "deltabox status"
                }
            ),

            ["commit"] = new CommandHelp(
                "commit <nome>",
                "Cria uma nova versão (commit) com um nome.",
                "deltabox commit <nome>",
                examples: new[]
                {
                    "deltabox commit \"cria tela de login\"",
                    "deltabox commit v1.0.0"
                }
            ),

            ["log"] = new CommandHelp(
                "log",
                "Lista commits e mostra a branch atual.",
                "deltabox log",
                examples: new[]
                {
                    "deltabox log"
                }
            ),

            ["remove"] = new CommandHelp(
                "remove <branch>",
                "Remove uma branch.",
                "deltabox remove <branch>",
                examples: new[]
                {
                    "deltabox remove feature-login"
                }
            ),

            ["branch"] = new CommandHelp(
                "branch <nome>",
                "Cria uma nova branch.",
                "deltabox branch <nome>",
                examples: new[]
                {
                    "deltabox branch feature-login"
                }
            ),

            ["checkout"] = new CommandHelp(
                "checkout <nome>",
                "Troca para outra branch.",
                "deltabox checkout <nome>",
                examples: new[]
                {
                    "deltabox checkout main",
                    "deltabox checkout feature-login"
                }
            ),

            ["merge"] = new CommandHelp(
                "merge <branch>",
                "Faz merge da <branch> na branch atual (somente quando atual = main).",
                "deltabox merge <branch>",
                rules: new[]
                {
                    "Só é permitido executar merge quando a branch atual for \"main\".",
                    "O merge traz as mudanças da <branch> para a main."
                },
                examples: new[]
                {
                    "deltabox checkout main",
                    "deltabox merge feature-login"
                }
            ),
        };
    }
    
    private void PrintGeneralHelp()
    {
        Console.WriteLine("DeltaBox — versionamento simples por pastas");
        Console.WriteLine();
        Console.WriteLine("USO:");
        Console.WriteLine("  deltabox <comando> [opções]");
        Console.WriteLine();
        Console.WriteLine("COMANDOS:");

        // ordena e alinha bonitinho
        var rows = _help
            .Select(kv => new { Name = kv.Key, Usage = kv.Value.Usage, Desc = kv.Value.ShortDescription })
            .OrderBy(x => x.Name)
            .ToList();

        var left = rows.Max(r => r.Usage.Length) + 2;

        foreach (var r in rows)
            Console.WriteLine($"  {r.Usage.PadRight(left)}{r.Desc}");

        Console.WriteLine();
        Console.WriteLine("DICA:");
        Console.WriteLine("  deltabox help <comando>  Mostra detalhes e exemplos.");
    }

    private static void PrintCommandHelp(CommandHelp info)
    {
        Console.WriteLine("COMANDO:");
        Console.WriteLine($"  {info.Name}");
        Console.WriteLine();
        Console.WriteLine("DESCRIÇÃO:");
        Console.WriteLine($"  {info.ShortDescription}");

        if (info.Rules?.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine("REGRAS:");
            foreach (var rule in info.Rules)
                Console.WriteLine($"  • {rule}");
        }

        Console.WriteLine();
        Console.WriteLine("USO:");
        Console.WriteLine($"  {info.Syntax}");

        if (info.Examples?.Length > 0)
        {
            Console.WriteLine();
            Console.WriteLine("EXEMPLOS:");
            foreach (var ex in info.Examples)
                Console.WriteLine($"  {ex}");
        }
    }

    private sealed record CommandHelp(
        string Usage,
        string ShortDescription,
        string Syntax,
        string[]? rules = null,
        string[]? examples = null)
    {
        public string Name => Usage.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        public string[]? Rules => rules;
        public string[]? Examples => examples;
    }

    public Result Execute(CommandContext ctx)
    {
        // help ou help <comando>
        if (ctx.Args.Length == 0)
        {
            PrintGeneralHelp();
            return Result.Success();
        }

        var cmd = ctx.Args[1];
        if (!_help.TryGetValue(cmd, out var info))
        {
            Console.WriteLine($"Comando desconhecido: {cmd}");
            Console.WriteLine();
            PrintGeneralHelp();
            return Result.Success();
        }

        PrintCommandHelp(info);
        return Result.Success();

    }
}