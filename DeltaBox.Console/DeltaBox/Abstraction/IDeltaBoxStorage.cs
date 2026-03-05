using DeltaBox.Commum;

namespace DeltaBox.Abstraction;

public interface IDeltaBoxStorage
{
    Result ChangeCurrentVersion(string path, string versionCurrent);
    Result ChangeCurrentBranch(string path, string branchCurrent);
    Result AddNewBranch(string path, string branchBase,string versionBase,string newBranchName);
    Result AddNewVersion(string path, string branchCurrent,string versionName);
}