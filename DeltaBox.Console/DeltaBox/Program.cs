using DeltaBox.Services;

var create = new InitVersionHandler();
create.Create(args.FirstOrDefault());

var nameFile = Console.ReadLine();
var alter = new AddVersionsHandler();
alter.Create(args.FirstOrDefault(),nameFile);


static void ViewAltersFiles(string pathFromFolder)
{
    var files = Directory.GetFiles(pathFromFolder);
    if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
        return ;
    var result = new Dictionary<string, string>();

    foreach (var line in File.ReadLines(pathFromFolder + "/deltabox"))
    {
        var parts = line.Split('|');
        if (parts.Length == 3) result[parts[1]] = parts[2];
    }
    
    foreach (var filePath in files)
    {
        byte[] currentContent = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        var key = result.Keys.FirstOrDefault(x => x == fileName);
        if (key is not null)
        {
            byte[] newVersionFile = new byte[currentContent.Length];
            var fileInBytePrevius = Convert.FromBase64String(result[key]);

            for (var b = 0; b < currentContent.Length; b++)
            {
                if (b >= fileInBytePrevius.Length)
                {
                }
                else
                {
                    if (fileInBytePrevius[b] != currentContent[b])
                        Console.WriteLine(fileName);
                }
            }
        }
        if(key is null)
        {
            Console.WriteLine(fileName);
        }
    }
}