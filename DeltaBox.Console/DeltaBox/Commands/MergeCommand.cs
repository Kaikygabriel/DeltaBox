using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class MergeCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        return Merge(ctx.Folder,ctx.Args[1],ctx.Args[2]);
    }

    public Result Merge(string folder , string branch,string nameVersionMerge)
    {
        var files = Directory.GetFiles(folder);
        if (!files.Any(x => Path.GetFileName(x) == Configure.DeltaBoxFile))
            return Error.DeltaBoxNotFound();
        
        var fileDeltaBox = folder + "/"+Configure.DeltaBoxFile;

        var deltaBox = File.ReadAllLines(fileDeltaBox); 
        var versionBaseBranch = "";
        var branchBase = "main";
        foreach (var d in deltaBox)
        {
            var parts = d.Split('|');
            if (parts[0].Equals("Branch") && parts[1].Equals(branch))
            {
                branchBase = parts[2];
                versionBaseBranch = parts[3];
            }
        }

        if (string.IsNullOrEmpty(versionBaseBranch))
            return new Error("Branch.NotFound", "Not Found Branch !");

        var filesInVersionBaseVersion = GetFilesInDeltaBox(deltaBox, branchBase, versionBaseBranch);

        var finishVersionBranchMerge = GetFinishVersion(branch,deltaBox);
        var finishVersionBranch = GetFinishVersion(branchBase,deltaBox);
        
        var filesInFinishVersionBranchMerge = GetFilesInDeltaBox(deltaBox, branch, finishVersionBranchMerge);
        var filesInFinishVersionBranch = GetFilesInDeltaBox(deltaBox, branchBase, finishVersionBranch);


        foreach (var f in filesInVersionBaseVersion)
        {
            if (!filesInFinishVersionBranch.ContainsKey(f.Key) &&
                !filesInFinishVersionBranchMerge.ContainsKey(f.Key))
            {
                
            }
            else if (filesInFinishVersionBranch.ContainsKey(f.Key) &&
                     !filesInFinishVersionBranchMerge.ContainsKey(f.Key))
            {
                filesInVersionBaseVersion[f.Key] = filesInFinishVersionBranch.GetValueOrDefault(f.Key)??f.Value;
            }
            else if (!filesInFinishVersionBranch.ContainsKey(f.Key) &&
                     filesInFinishVersionBranchMerge.ContainsKey(f.Key))
            {
                filesInVersionBaseVersion[f.Key] = filesInFinishVersionBranchMerge.GetValueOrDefault(f.Key)??f.Value;
            }
            else if (filesInFinishVersionBranch.ContainsKey(f.Key) &&
                     filesInFinishVersionBranchMerge.ContainsKey(f.Key))
            {
                var fileBaseVersion = Convert.FromBase64String(f.Value);
            
                var fileBranchMerge =Convert.FromBase64String(filesInFinishVersionBranchMerge[f.Key]??f.Value);
                var fileBranch = Convert.FromBase64String(filesInFinishVersionBranch[f.Key]??f.Value);

                if (fileBaseVersion.SequenceEqual(fileBranchMerge) && fileBaseVersion.SequenceEqual(fileBranch))
                {
                        
                }
                else if (fileBaseVersion.SequenceEqual(fileBranchMerge) && !fileBaseVersion.SequenceEqual(fileBranch))
                {
                    filesInVersionBaseVersion[f.Key] = Convert.ToBase64String(fileBranch);
                }
                else if (!fileBaseVersion.SequenceEqual(fileBranchMerge) && fileBaseVersion.SequenceEqual(fileBranch))
                {
                    filesInVersionBaseVersion[f.Key] = Convert.ToBase64String(fileBranchMerge);
                }
                else if (!fileBaseVersion.SequenceEqual(fileBranchMerge) && !fileBaseVersion.SequenceEqual(fileBranch))
                {

                    var fileBranchMergeText = Encoding.UTF8.GetString(fileBranchMerge);
                    var fileBranchText = Encoding.UTF8.GetString(fileBranch);
                    
                    var linesMerge = fileBranchMergeText.Split(Environment.NewLine);
                    var linesOriginal = fileBranchText.Split(Environment.NewLine);
                    var maxLines = Math.Max(linesMerge.Length, linesOriginal.Length);

                    var builder = new StringBuilder();

                    for (int i = 0; i < maxLines; i++)
                    {
                        var lineMerge = i < linesMerge.Length ? linesMerge[i] : string.Empty;
                        var lineOriginal = i < linesOriginal.Length ? linesOriginal[i] : string.Empty;

                        if (lineMerge == lineOriginal)
                        {
                            builder.AppendLine(lineMerge);
                        }
                        else
                        {
                            if (lineOriginal != string.Empty)
                                builder.AppendLine(lineOriginal+" -- "+branchBase+ "\n");

                            if (lineMerge != string.Empty)
                                builder.AppendLine(lineMerge +" -- "+branch);
                        }
                    }

                    var mergedText = builder.ToString();
                    var mergedBytes = Encoding.UTF8.GetBytes(mergedText);
                    filesInVersionBaseVersion[f.Key] = Convert.ToBase64String(mergedBytes);
                }
                
            }

            if (filesInFinishVersionBranch.ContainsKey(f.Key))
                filesInFinishVersionBranch.Remove(f.Key);

            if (filesInFinishVersionBranchMerge.ContainsKey(f.Key))
                filesInFinishVersionBranchMerge.Remove(f.Key);
        }

        foreach (var merge in filesInFinishVersionBranchMerge)
        {
            if (filesInFinishVersionBranch.ContainsKey(merge.Key))
            {
                var fileBranchMerge =Convert.FromBase64String(merge.Value);
                var fileBranch = Convert.FromBase64String(filesInFinishVersionBranch[merge.Key]);

                var fileBranchMergeText = Encoding.UTF8.GetString(fileBranchMerge);
                var fileBranchText = Encoding.UTF8.GetString(fileBranch);
                    
                var linesMerge = fileBranchMergeText.Split(Environment.NewLine);
                var linesOriginal = fileBranchText.Split(Environment.NewLine);
                var maxLines = Math.Max(linesMerge.Length, linesOriginal.Length);

                var builder = new StringBuilder();

                for (int i = 0; i < maxLines; i++)
                {
                    var lineMerge = i < linesMerge.Length ? linesMerge[i] : string.Empty;
                    var lineOriginal = i < linesOriginal.Length ? linesOriginal[i] : string.Empty;

                    if (lineMerge == lineOriginal)
                    {
                        builder.AppendLine(lineMerge);
                    }
                    else
                    {
                        if (lineOriginal != string.Empty)
                            builder.AppendLine(lineOriginal+" -- "+branchBase+ "\n");

                        if (lineMerge != string.Empty)
                            builder.AppendLine(lineMerge +" -- "+branch);
                    }
                }

                var mergedText = builder.ToString();
                var mergedBytes = Encoding.UTF8.GetBytes(mergedText);
                filesInVersionBaseVersion[merge.Key] = Convert.ToBase64String(mergedBytes);
            }
            else
            {
                var fileBranchMerge =Convert.FromBase64String(merge.Value);
                filesInVersionBaseVersion[merge.Key] = Convert.ToBase64String(fileBranchMerge);
            }
        }
        foreach (var branchother in filesInFinishVersionBranch)
        {
            if (filesInFinishVersionBranchMerge.ContainsKey(branchother.Key))
            {
                var fileBranchMerge =Convert.FromBase64String(branchother.Value);
                var fileBranch = Convert.FromBase64String(filesInFinishVersionBranchMerge[branchother.Key]);

                var fileBranchMergeText = Encoding.UTF8.GetString(fileBranchMerge);
                var fileBranchText = Encoding.UTF8.GetString(fileBranch);
                    
                var linesMerge = fileBranchMergeText.Split(Environment.NewLine);
                var linesOriginal = fileBranchText.Split(Environment.NewLine);
                var maxLines = Math.Max(linesMerge.Length, linesOriginal.Length);

                var builder = new StringBuilder();

                for (int i = 0; i < maxLines; i++)
                {
                    var lineMerge = i < linesMerge.Length ? linesMerge[i] : string.Empty;
                    var lineOriginal = i < linesOriginal.Length ? linesOriginal[i] : string.Empty;

                    if (lineMerge == lineOriginal)
                    {
                        builder.AppendLine(lineMerge);
                    }
                    else
                    {
                        if (lineOriginal != string.Empty)
                            builder.AppendLine(lineOriginal+" -- "+branchBase+ "\n");

                        if (lineMerge != string.Empty)
                            builder.AppendLine(lineMerge +" -- "+branch);
                    }
                }

                var mergedText = builder.ToString();
                var mergedBytes = Encoding.UTF8.GetBytes(mergedText);
                filesInVersionBaseVersion[branchother.Key] = Convert.ToBase64String(mergedBytes);
            }
            else
            {
                var fileBranchMerge =Convert.FromBase64String(branchother.Value);
                filesInVersionBaseVersion[branchother.Key] = Convert.ToBase64String(fileBranchMerge);
            }
        }

        File.AppendAllText(fileDeltaBox,$"\n{branchBase}|{nameVersionMerge}|{DateTime.UtcNow}\n");
        foreach (var saveFile in filesInVersionBaseVersion)
        {
            var text = $"{branchBase}|{nameVersionMerge}|{saveFile.Key}|{saveFile.Value}\n";
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                text = $"{branchBase}|{nameVersionMerge}|{saveFile.Key.Replace('\\','/')}|{saveFile.Value}\n";
            Console.WriteLine($"Versionando :  {text}");
            File.AppendAllText(fileDeltaBox, text);
            
        }
        return Result.Success();
    }
    
    private Dictionary<string, string> GetFilesInDeltaBox(string[] deltaBox, string branch, string version)
    {
        var filesInVersionBaseBranch = new Dictionary<string, string>();
        foreach (var d in deltaBox)
        {
            var parts = d.Split('|');
            if (parts.Length == 4&&parts[0].Equals(branch) && parts[1].Equals(version))
                filesInVersionBaseBranch.Add(parts[2],parts[3]);
        }

        return filesInVersionBaseBranch;
    }

    private string GetFinishVersion(string branch,string[] deltaBox)
    {
        var finishVersion = "";
        foreach (var d in deltaBox)
        {
            var parts = d.Split('|');
            if (parts.Length == 4 && parts[0].Equals(branch))
                finishVersion = parts[1];
        }

        return finishVersion;
    }
}