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
            if ( Path.GetFileName(file)!= "deltabox")
                File.Delete(file);
        
        var dictionaries = Directory.GetDirectories(
            pathFromFolder);
        foreach(var d in dictionaries)
            Console.WriteLine(d);
        
        if (dictionaries.Length > 0)
            RemoveFiles(dictionaries, pathFromFolder);
        
        foreach (var result in results)
        {
            var fileInBytePrevius = Convert.FromBase64String(result.Value);
            var folders = result.Key;
            var segregationsFolders = folders.Split('\\').ToList();
             var corte1 =
               segregationsFolders
                   .IndexOf(segregationsFolders.First(x=>x == Path.GetDirectoryName(pathFromFolder.Remove(0,2))));
            var corte2=  segregationsFolders
                .IndexOf(segregationsFolders.Last());
            var path = ".";
            if (corte2 - corte1 >= 1)
            {
                 var nemList = segregationsFolders[corte1..corte2];
                 foreach (var s in nemList)
                 {
                     path =$"{path}/{s}";
                     Directory.CreateDirectory(path);
                 }
            }

            if (result.Key != "deltabox")
                File.WriteAllBytes(result.Key,fileInBytePrevius);
            
        }
    }

    private static void RemoveFiles(string[] dictionaries,string pathFromFolder)
    {
        foreach (var subDictionary in dictionaries)
        {
            if (!subDictionary.Equals(pathFromFolder))
            {
                var fileSubDictionary = Directory.GetFiles(subDictionary);
                foreach (var file in fileSubDictionary)
                {
                    if ( Path.GetFileName(file)!= "deltabox")
                        File.Delete(file);
                }
                var subSubDictionary = Directory.GetDirectories(
                    subDictionary);
                if (subSubDictionary.Length > 0)
                {
                    RemoveFiles(subSubDictionary, pathFromFolder);
                }
                if(subDictionary != pathFromFolder)
                    Directory.Delete(subDictionary);    
            }
        } 
    }
}