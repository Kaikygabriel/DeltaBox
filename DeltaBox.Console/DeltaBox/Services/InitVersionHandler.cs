namespace DeltaBox.Services;

public class InitVersionHandler
{
    public bool Create(string pathOfFiles)
    {
        TryInitVersionInFolder(pathOfFiles);
        return true;
    }

    private bool TryInitVersionInFolder(string pathFromFolder)
    {
        try
        {
            if (!Directory.Exists(pathFromFolder))
                return false;

            var files = Directory.GetFiles(pathFromFolder);
            
            SaveFilesOfDirectory(files,pathFromFolder);
            
            var dictionaries = Directory.GetDirectories(
                pathFromFolder,
                "*",
                SearchOption.AllDirectories);

            foreach (var subDictionary in dictionaries)
            {
                var filesInSubDictionary = Directory.GetFiles(subDictionary);
                SaveFilesOfDirectory(filesInSubDictionary,pathFromFolder);
            }  
          
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private void SaveFilesOfDirectory(string[] files,string pathFromFolder)
    {
        var fileDeltaBox = pathFromFolder + "/deltabox";
        for (var a = 0; a < files.Length; a++)
        {
            byte[] content = File.ReadAllBytes(files[a]);
            var fileInBase64 = Convert.ToBase64String(content);
            var text = $"Init|{files[a]}|{fileInBase64}\n";
            Console.WriteLine($"Versionando :  {text}");
            File.AppendAllText(fileDeltaBox, text);
        }
    }
}