using System.Net;

namespace DeltaBox.Services;

public class VersionsHandler
{
    public static void GoToVersion1(string pathFromFolder, string versionName)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
        { 
            return;
        } 
        
        var results = new Dictionary<string, string>();
            
        foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 3 && parts[0].Equals(versionName,StringComparison.InvariantCultureIgnoreCase))
            {
                if (parts[0].Equals(versionName,StringComparison.InvariantCultureIgnoreCase))
                {
                    results.Add(parts[1],parts[2]);
                }
            }
        }

        if (results.Count <= 0)
            return ;
        
        foreach (var file in files)
        {
            if ( Path.GetFileName(file)!= "deltabox")
                File.Delete(file);
        }
        
        foreach (var result in results)
        {
            var fileInBytePrevius = Convert.FromBase64String(result.Value);

            if (result.Key != "deltabox")
            {
                File.WriteAllBytes(pathFromFolder+"/"+result.Key,fileInBytePrevius);
            }
        }
    }

}