namespace DeltaBox.Abstraction;

public interface IDeltaBoxStorage
{
    void ChangeCurrentVersion(string path, string versionCurrent);
    void ChangeCurrentBranch(string path, string branchCurrent);
    void AddNewBranch(string path, string branchBase,string versionBase,string newBranchName);
    void AddNewVersion(string path, string branchCurrent,string versionName);
}