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
        var pathDeltaBox = Path.Combine(path, Configure.DeltaBoxFile);
        if (!File.Exists(pathDeltaBox))
            return Error.DeltaBoxNotFound();
        var filesInDeltaBox = File.ReadAllLines(pathDeltaBox);

        File.SetAttributes(pathDeltaBox, FileAttributes.Normal);

        filesInDeltaBox[0] = $"CurrentVersion|{versionCurrent}";

        _fileSystem.WriteAll(pathDeltaBox, filesInDeltaBox);
        
        return Result.Success();
    }

    public Result ChangeCurrentBranch(string path, string branchCurrent)
    {
        var pathDeltaBox = Path.Combine(path, Configure.DeltaBoxFile);
        if (!File.Exists(pathDeltaBox))
            return Error.DeltaBoxNotFound();
        var filesInDeltaBox = File.ReadAllLines(pathDeltaBox);

        File.SetAttributes(pathDeltaBox, FileAttributes.Normal);

        filesInDeltaBox[2] = $"BranchCurrent|{branchCurrent}";

        _fileSystem.WriteAll(pathDeltaBox, filesInDeltaBox);
        return Result.Success();
    }

    public Result AddNewBranch(string path, string branchBase, string versionBase, string newBranchName)
    {
        var pathDeltaBox = Path.Combine(path, Configure.DeltaBoxFile);
        if (!File.Exists(pathDeltaBox))
            return Error.DeltaBoxNotFound();

        _fileSystem.Append(pathDeltaBox, $"\nBranch|{newBranchName}|{branchBase}|{versionBase}\n");
        return Result.Success();
    }

    public Result AddNewVersion(string path, string branchCurrent, string versionName)
    {
        var pathDeltaBox = Path.Combine(path, Configure.DeltaBoxFile);
        if (!File.Exists(pathDeltaBox))
            return Error.DeltaBoxNotFound();

        _fileSystem.Append(pathDeltaBox, $"\n{branchCurrent}|{versionName}|{DateTime.UtcNow}\n");
        return Result.Success();
    }
}