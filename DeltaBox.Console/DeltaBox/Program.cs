using DeltaBox.Abstraction;
using DeltaBox.Commum;
using DeltaBox.Services;
using DeltaBox.View;

var folder = args[1] ?? throw new Exception("Method Invalid");
var method = args[0]?? throw new Exception("Method Invalid");

var commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
{
    ["init"] = new InitVersionHandler(),
    ["previus"] = new VersionsHandler(),
    ["status"] = new AltersFilesHandler(),
    ["commit"] = new AddVersionsHandler(),
    ["log"] = new GetVersionsHandler(),
    ["remove"] = new RemoveVersionHandler(),
};

if (!commands.TryGetValue(method, out var command))
{
    Console.WriteLine($"Command Invalid: {method}");
    return;
}

var resultCommand = command.Execute(new CommandContext(folder, args));
if (!resultCommand.IsSuccess)
    ViewError.Get(resultCommand.Error);