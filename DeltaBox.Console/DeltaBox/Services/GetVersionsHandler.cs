namespace DeltaBox.Services;

public class GetVersionsHandler
{
    public static void Get(string pathFromFolder)
    {
        Console.WriteLine(" \nVersions :\n");

        if (!Directory.Exists(pathFromFolder))
            return ;
        var files = Directory.GetFiles(pathFromFolder);
        var deltaBoxFile = files.FirstOrDefault(x => Path.GetFileName(x).Equals("deltabox"));
        if (deltaBoxFile is null)
            return ;
        foreach (var line in File.ReadLines(deltaBoxFile))
        {
            var parts = line.Split('|');
            if (parts.Length == 2)
            {
                Console.WriteLine($"\t{parts[0]} - {parts[1]}");
            }
        }
    }
}