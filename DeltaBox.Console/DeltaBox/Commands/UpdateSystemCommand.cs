using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

public class UpdateSystemCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        if (!Directory.Exists(ctx.Folder))
            return Error.DirectoryNotFound();
        var files = Directory.GetFiles(ctx.Folder);

        if (!files.Any(x => Path.GetFileName(x).Equals("deltabox")))
            return new Error("DeltaBox.No.Exists", "File deltabox No Exists!");
        
        var fileDeltaBox = ctx.Folder + "/deltabox";
        
        var lines = File.ReadLines(fileDeltaBox).ToArray();
        for (var line = 0; line < lines.Length; line++)
        {
            var parts = lines[line].Split('|');
            if (parts.Length >= 4 && parts[0]!="Branch")
            {
                if (OperatingSystem.IsWindows())
                {
                    var text = $"{parts[0]}|{parts[1]}|{parts[2].Replace('/','\\')}|{parts[3]}";
                    lines[line] = text;
                }
                else if (OperatingSystem.IsMacOS()|| OperatingSystem.IsLinux())
                {
                    var text = $"{parts[0]}|{parts[1]}|{parts[2].Replace('\\','/')}|{parts[3]}";
                    lines[line] = text;
                }
            }
        }
        File.WriteAllLines(fileDeltaBox,lines);
        return Result.Success();
    }
}