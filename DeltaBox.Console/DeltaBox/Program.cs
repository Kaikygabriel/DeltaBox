using DeltaBox.Services;

var create = new InitVersionHandler();
create.Create(args.FirstOrDefault());
Console.ReadLine();
AltersFilesHandler.ViewAltersFiles(args.FirstOrDefault());
var nameFile = Console.ReadLine();
var alter = new AddVersionsHandler();
alter.Create(args.FirstOrDefault(),nameFile);

