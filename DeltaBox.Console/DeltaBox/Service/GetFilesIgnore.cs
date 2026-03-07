using DeltaBox.Commum;

namespace DeltaBox.Service;

public class GetFilesIgnore
{
    private List<string> _filesIgnore = [];
    private List<string> _extensionsIgnore = [];

    public bool FilesIgnore(string nameFile,string path)
    {
        if(_filesIgnore.Count <=0|| _filesIgnore.Count <=0)
            Init(path);

        if (nameFile.Equals(Configure.DeltaBoxIgnoreFile))
            return true;
        if (_filesIgnore.Exists(x => x.Equals(nameFile)))
            return true;
        if (_extensionsIgnore.Exists(x => nameFile.Split('.').Last().Equals(x.Split('.').Last())))
            return true;
        return false;
    }

    public void Init(string path)
    {
        var files = File.ReadAllLines(path);
        foreach (var f in files)
        {
            if(f.Contains("*."))
                _extensionsIgnore.Add(f);
            
            _filesIgnore.Add(f);
        }
    }
}