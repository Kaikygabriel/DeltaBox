using DeltaBox.Services;

var create = new InitVersionHandler();
//create.Create(args.FirstOrDefault());

//Console.ReadLine();
//AltersFilesHandler.ViewAltersFiles(args.FirstOrDefault());

//var nameFile = Console.ReadLine();
var alter = new AddVersionsHandler();
//alter.Create(args.FirstOrDefault(),nameFile);

//VersionsHandler.GoToVersion1(args.FirstOrDefault(),nameFile);


var folder = args[1];
var method = args[0];


if(string.IsNullOrEmpty(folder) || string.IsNullOrWhiteSpace(method))
    Console.WriteLine("Nenhum metodo ativo ");

else if(method.Equals("Init",StringComparison.CurrentCultureIgnoreCase))
    create.Create(folder);

else if (method.Equals("Previus",StringComparison.CurrentCultureIgnoreCase))
    VersionsHandler.GoToVersion1(folder,args[2]);

else if (method.Equals("Status",StringComparison.CurrentCultureIgnoreCase))
    AltersFilesHandler.ViewAltersFiles(folder);

else if (method.Equals("Commit",StringComparison.CurrentCultureIgnoreCase) ) 
    alter.Create(folder,args[2]);

