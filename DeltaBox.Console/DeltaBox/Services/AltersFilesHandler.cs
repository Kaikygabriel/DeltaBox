namespace DeltaBox.Services;

public static class AltersFilesHandler
{
    
    public static void ViewAltersFiles(string pathFromFolder)
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
                var fileInBytePrevius = Convert.FromBase64String(result[key]);
                if (!currentContent.SequenceEqual(fileInBytePrevius))
                {
                    var currentTextColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("- "+fileName);
                    Console.ForegroundColor = currentTextColor;
                }
            }
            if(key is null && fileName != "deltabox")
            {
                var currentTextColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("+ "+fileName);
                Console.ForegroundColor = currentTextColor;
            }
        }
    }
}