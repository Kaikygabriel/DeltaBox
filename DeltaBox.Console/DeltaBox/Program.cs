using DeltaBox.Abstraction;
using DeltaBox.Commands;
using DeltaBox.Commum;
using DeltaBox.View;

 try 
 {
    var folder = Environment.CurrentDirectory;

    var method = "";
    if (args is not null && args.Any())
    {
        method = args[0];
    }

    var commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
    {
        ["help"] = new HelpCommand(),
        ["init"] = new InitVersionCommand(),
        ["prev"] = new VersionsCommand(),
        ["status"] = new AltersFilesCommand(),
        ["commit"] = new AddVersionsCommand(),
        ["log"] = new GetVersionsCommand(),
        ["remove"] = new RemoveVersionCommand(),
        ["branch"] = new CreateBranchCommand(),
        ["checkout"] = new CheckoutCommand(),
        ["merge"] = new MergeCommand(),
        ["push"] = new PushCommand(),
        ["pull"] = new PullCommand(),
        ["connect"] = new ConnectGitHubCommand()
    };

    if (!commands.TryGetValue(method, out var command))
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            commands["help"].Execute(new CommandContext(folder, new List<string> { "", "" }.ToArray()));
        }

        Console.Error.WriteLine($"Command Invalid: {method}");
        Environment.Exit(1);
        return;
    }

    var resultCommand = command.Execute(new CommandContext(folder, args));
    if (!resultCommand.IsSuccess)
        ViewError.Get(resultCommand.Error);
}
catch (Exception e)
{
     Console.Error.WriteLine("Error");
     Environment.Exit(1);
}