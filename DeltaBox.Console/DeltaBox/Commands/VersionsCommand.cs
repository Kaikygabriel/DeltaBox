using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class VersionsCommand : ICommand
{
    public Result GoToVersion(string pathFromFolder, string versionName)
    {
        var files = Directory.GetFiles(pathFromFolder);
        if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
            return Error.DeltaBoxNotFound();
        
        var currentBranch = ""; 
        foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
        {
            var parts = line.Split('|');
            if (parts[0].Equals("BranchCurrent"))
            {
                currentBranch = parts[1];
            }
        }
        
        
        var results = new Dictionary<string, string>();
            
        foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 4 && parts[1].Equals(versionName,StringComparison.InvariantCultureIgnoreCase))
            {
                if (parts[0] != currentBranch)
                    return new Error("Branch.Invalid", "The branch  is different");
                
                results.Add(parts[2],parts[3]);
            }
        }

        if (results.Count <= 0)
            return new Error("Version.NotFound","Version not found!");
        VerifyAltersFile(files, pathFromFolder + "/deltabox", pathFromFolder);
        foreach (var file in files)
            if ( Path.GetFileName(file)!= "deltabox")
                File.Delete(file);
        
        var dictionaries = Directory.GetDirectories(
            pathFromFolder);
        
        if (dictionaries.Length > 0)
            RemoveFiles(dictionaries, pathFromFolder);
        
        foreach (var result in results)
        {
            var fileInBytePrevius = Convert.FromBase64String(result.Value);
            var folders = result.Key;
            var segregationsFolders = folders.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            int corte1=0;
            if (OperatingSystem.IsWindows())
            { 
                corte1 =
                    segregationsFolders
                        .IndexOf(segregationsFolders.First(x=>x == Path.GetDirectoryName(pathFromFolder.Remove(0,2))));

            }
            else if (OperatingSystem.IsLinux()||OperatingSystem.IsMacOS())
            { 
                corte1 =
                    segregationsFolders
                        .IndexOf(segregationsFolders.First(x=>x == Path.GetDirectoryName(pathFromFolder)));

            }
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
            {
                if(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                    File.WriteAllBytes(result.Key.Replace('\\','/'),fileInBytePrevius);
                else
                    File.WriteAllBytes(result.Key,fileInBytePrevius);
            }

            var lines = File.ReadLines(pathFromFolder + "/deltabox").ToList();
            lines[0] = $"CurrentVersion|{versionName}";
            File.WriteAllLines(pathFromFolder + "/deltabox", lines);
    
        }

        return Result.Success();
    }

    public void VerifyAltersFile(IEnumerable<string>files,string deltaBoxFile,string path)
    {
        var versionFinish = "";
        foreach (var line in File.ReadLines(path + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 2)
            {
                versionFinish = parts.First();
            }
        }

        var currentVersion = "";
        foreach (var line in File.ReadLines(path + "/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 2 && parts[0] == "CurrentVersion")
            {
                currentVersion = parts[1];
            }
        }
        
        if (!versionFinish.Equals(currentVersion))
            return;

        var result = new Dictionary<string, string>();
            
        foreach (var line in File.ReadLines(path+"/deltabox"))
        {
            var parts = line.Split('|');
            if (parts.Length == 4 && parts[1].Equals(currentVersion,StringComparison.InvariantCultureIgnoreCase))
            {
                result.Add(parts[2],parts[3]);
            }
        }
        var dictionaries = Directory.GetDirectories(
            path,
            "*",
            SearchOption.AllDirectories);
        
        var fp = Directory.GetFiles(path);
        foreach (var fileNew in fp)
        {
            byte[] currentContent = File.ReadAllBytes(fileNew);

            var key = result.Keys.FirstOrDefault(x => x.Equals(fileNew));
            string fileName = Path.GetFileName(fileNew);
            if (key is not null)
            {
                var fileInBytePrevius = Convert.FromBase64String(result[key]);
                if (!currentContent.SequenceEqual(fileInBytePrevius))
                {
                    if (!MessageIfAltersOrNewFiles())
                    {
                        break;
                        return;
                    }
                }
            }
            else if (key is null && fileName != "deltabox")
            {
                if (!MessageIfAltersOrNewFiles())
                    throw new Exception("Pending files: commit to save before reverting to a previous version.");
            }
        }

        foreach (var d in dictionaries)
        {
            var filesPath = Directory.GetFiles(d);
            foreach (var fileNew in filesPath)
            {
                byte[] currentContent = File.ReadAllBytes(fileNew);

                var key = result.Keys.FirstOrDefault(x => x.Equals(fileNew));
                string fileName = Path.GetFileName(fileNew);
                if (key is not null)
                {
                    var fileInBytePrevius = Convert.FromBase64String(result[key]);
                    if (!currentContent.SequenceEqual(fileInBytePrevius))
                    {
                        if (!MessageIfAltersOrNewFiles())
                        {
                            throw new Exception("Pending files: commit to save before reverting to a previous version.");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else if(key is null && fileName != "deltabox")
                {
                    if(!MessageIfAltersOrNewFiles())
                        throw new Exception("Pending files: commit to save before reverting to a previous version.");
                    else
                    {
                        return;
                    }
                }
            }
        }
    }

    private bool MessageIfAltersOrNewFiles()
    {
        Console.WriteLine("Are you sure you want to change versions? There are unsaved files. [S] Yes or [N] No");
        var response = Console.ReadLine();
        if (response is null)
            return false; 
        if (response.Equals("s", StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }
        
        return false;
    }
    
    private void RemoveFiles(string[] dictionaries,string pathFromFolder)
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


    public Result Execute(CommandContext ctx)
    {
        if (ctx.Args[2] is null)
            return Error.CommandInvalid();
        return GoToVersion(ctx.Folder, ctx.Args[2]);
    }
}