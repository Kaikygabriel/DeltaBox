using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Storage;

public class DeltaBoxStorage : IDeltaBoxStorage
{
    private readonly IFileSystem _fileSystem;

    public DeltaBoxStorage(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public Result ChangeCurrentVersion(string path, string versionCurrent)
    {
        if (!File.Exists(path))
        {
            _fileSystem.AppendText(path,$"CurrentVersion|{versionCurrent}\n");
            return Result.Success();
        }
        
        var  filesInDeltaBox = _fileSystem.ReadAllLines(path);
        
        File.SetAttributes(path, FileAttributes.Normal);

        if (filesInDeltaBox.Length <= 0)
        {
            _fileSystem.AppendText(path,$"CurrentVersion|{versionCurrent}");
        }
        else
        {
            filesInDeltaBox[0] = $"CurrentVersion|{versionCurrent}"; 
            _fileSystem.WriteAll(path, filesInDeltaBox);
        }
        
        return Result.Success();
    }

    public Result ChangeCurrentBranch(string path, string branchCurrent)
    {
        if (!File.Exists(path))
        {
            _fileSystem.AppendText(path,$"\nBranchCurrent|{branchCurrent}");
            return Result.Success();
        }
        
        var filesInDeltaBox = _fileSystem.ReadAllLines(path);
        
        File.SetAttributes(path, FileAttributes.Normal);

        if (filesInDeltaBox.Length <= 1)
        { 
            _fileSystem.AppendText(path,$"\nBranchCurrent|{branchCurrent}");
        }
        else
        {
            filesInDeltaBox[2] = $"BranchCurrent|{branchCurrent}";

            _fileSystem.WriteAll(path, filesInDeltaBox);
        }
        return Result.Success();
    }

    public Result AddNewBranch(string path, string branchBase, string versionBase, string newBranchName)
    { 
        if (!File.Exists(path))
            return Error.DeltaBoxNotFound();

        _fileSystem.AppendText(path, $"\nBranch|{newBranchName}|{branchBase}|{versionBase}\n");
        return Result.Success();
    }

    public Result AddNewVersion(string path, string branchCurrent, string versionName)
    {
        if (!File.Exists(path))
            return Error.DeltaBoxNotFound();

        _fileSystem.AppendText(path, $"\n{branchCurrent}|{versionName}|{DateTime.UtcNow}\n");
        return Result.Success();
    }
}