using DeltaBox.Services;
using DeltaBox.View;


var folder = args[1] ?? throw new Exception("Method Invalid");
var method = args[0]?? throw new Exception("Method Invalid");

if(string.IsNullOrEmpty(folder) || string.IsNullOrWhiteSpace(method))
    Console.WriteLine("Nenhum metodo ativo ");

else if (method.Equals("Init", StringComparison.CurrentCultureIgnoreCase))
{
    var create = new InitVersionHandler();
    var result = create.Create(folder);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

else if (method.Equals("Previus", StringComparison.CurrentCultureIgnoreCase))
{
    var handler = new VersionsHandler();
    var result = handler.GoToVersion(folder,args[2]);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

else if (method.Equals("Status", StringComparison.CurrentCultureIgnoreCase))
{
    var handler = new AltersFilesHandler();
    var result = handler.ViewAltersFiles(folder);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

else if (method.Equals("Commit", StringComparison.CurrentCultureIgnoreCase))
{
    var handler = new AddVersionsHandler();
    var result = handler.Create(folder, args[2]);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

else if (method.Equals("Log", StringComparison.CurrentCultureIgnoreCase))
{
    var handler = new GetVersionsHandler();
    var result = handler.Get(folder);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

else if (method.Equals("Remove", StringComparison.CurrentCultureIgnoreCase))
{
    var handler = new RemoveVersionHandler();
    var result = handler.RemoveVersion(folder,args[2]);
    if (!result.IsSuccess)
        ViewError.Get(result.Error);
}

