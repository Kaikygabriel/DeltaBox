using DeltaBox.Abstraction;
using DeltaBox.Commands;
using DeltaBox.Commum;
using DeltaBox.View;

var folder = args[1] ?? throw new Exception("Method Invalid");
var method = args[0]?? throw new Exception("Method Invalid");

var commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
{
    ["init"] = new InitVersionCommand(),
    ["previus"] = new VersionsCommand(),
    ["status"] = new AltersFilesCommand(),
    ["commit"] = new AddVersionsCommand(),
    ["log"] = new GetVersionsCommand(),
    ["remove"] = new RemoveVersionCommand(),
    ["branch"] = new CreateBranchCommand(),
    ["checkout"] = new CheckoutCommand()

};

if (!commands.TryGetValue(method, out var command))
{
    Console.WriteLine($"Command Invalid: {method}");
    return;
}

var resultCommand = command.Execute(new CommandContext(folder, args));
if (!resultCommand.IsSuccess)
    ViewError.Get(resultCommand.Error);