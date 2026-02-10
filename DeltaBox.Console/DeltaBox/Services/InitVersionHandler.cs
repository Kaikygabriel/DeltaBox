using DeltaBox.Commum;

namespace DeltaBox.Services;

public class InitVersionHandler
{
    public Result Create(string pathOfFiles)
    {
        return InitVersionInFolder(pathOfFiles);
    }

    private Result InitVersionInFolder(string pathFromFolder)
    {
        if (!Directory.Exists(pathFromFolder))
            return Error.DirectoryNotFound();
        var files = Directory.GetFiles(pathFromFolder);

        if (files.Any(x => Path.GetFileName(x).Equals("deltabox")))
            return new Error("DeltaBox.AlreadyExists", "File deltabox already exists!");

        File.AppendAllText(pathFromFolder + "/deltabox", $"\nCurrentVersion|Init\n");
        File.AppendAllText(pathFromFolder + "/deltabox", $"\nInit|{DateTime.UtcNow}\n");

        SaveFilesOfDirectory(files, pathFromFolder);

        var dictionaries = Directory.GetDirectories(
            pathFromFolder,
            "*",
            SearchOption.AllDirectories);

        foreach (var subDictionary in dictionaries)
        {
            var filesInSubDictionary = Directory.GetFiles(subDictionary);
            SaveFilesOfDirectory(filesInSubDictionary, pathFromFolder);
        }

        return Result.Success();
    }

    private void SaveFilesOfDirectory(string[] files,string pathFromFolder)
    {
        var fileDeltaBox = pathFromFolder + "/deltabox";
        for (var a = 0; a < files.Length; a++)
        {
            byte[] content = File.ReadAllBytes(files[a]);
            var fileInBase64 = Convert.ToBase64String(content);
            var text = $"Init|{files[a]}|{fileInBase64}\n";
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                text = $"Init|{files[a].Replace('\\','/')}|{fileInBase64}\n";
            Console.WriteLine($"Versionando :  {text}");
            File.AppendAllText(fileDeltaBox, text);
        }
    }
}