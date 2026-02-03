using System.Text;

namespace DeltaBox.Services;

public class AddVersionsHandler
{
    public bool Create(string pathOfFiles,string nameVersion)
    {
        AddNewVersion(pathOfFiles,nameVersion);
        return true;
    }

    private bool AddNewVersion(string pathFromFolder,string nameVersion)
    {
        
            var files = Directory.GetFiles(pathFromFolder);
            if (!files.Any(x => Path.GetFileName(x) == "deltabox"))
                return false;
            var result = new Dictionary<string, string>();
            
            foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
            {
                var parts = line.Split('|');
                if (parts.Length == 3 && parts[0] == "Init")
                    result[parts[1]] = parts[2];
            }
            File.AppendAllText(pathFromFolder + "/deltabox", $"\n{nameVersion}\n");

            foreach (var filePath in files)
            {
                byte[] currentContent = File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);

                var key = result.Keys.FirstOrDefault(x => x.Equals(fileName,StringComparison.CurrentCultureIgnoreCase));
                if (result.Keys.Contains(fileName) && fileName != "deltabox")
                {
                    byte[] newVersionFile = [];
                    var teste = result.TryGetValue(fileName,out string byteeafd);
                    var fileInBytePrevius = Convert.FromBase64String(byteeafd); 

                    for (var b = 0; b < currentContent.Length; b++)
                    {
                        if (b >= fileInBytePrevius.Length)
                        {
                        }
                        else
                        {
                            if (fileInBytePrevius[b]!=currentContent[b])
                            {
                                Console.WriteLine("Passou por Aqui");
                                newVersionFile[b] = (byte)(fileInBytePrevius[b] - currentContent[b]);
                            }
                        }
                    }

                    if (fileInBytePrevius.Length > 0)
                    {
                            var text = $"{nameVersion}|{fileName}|{Convert.ToBase64String(newVersionFile)}\n";
                            Console.WriteLine($"Versionando : {text} ");
                            File.AppendAllText(pathFromFolder + "/deltabox", text);    
                             
                    }
                    

                }
                else if ( fileName != "deltabox"&& !result.Keys.Contains(fileName))
                {
                    byte[] content = File.ReadAllBytes(filePath);
                    var fileInBase64 = Convert.ToBase64String(content);
                    var text = $"{nameVersion}|{fileName}|{fileInBase64}\n";
                    Console.WriteLine($"Versionando : {text} ");
                    File.AppendAllText(pathFromFolder + "/deltabox", text);
                }
            }
            var sla = new Dictionary<string, string>();
            
            foreach (var line in File.ReadLines(pathFromFolder+"/deltabox"))
            {
                var parts = line.Split('|'); 
                if (parts.Length == 3) sla[parts[1]] = parts[2];
            }
            
                        
            foreach(var r in sla)
                Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(r.Value)));

            return true;
    }
}