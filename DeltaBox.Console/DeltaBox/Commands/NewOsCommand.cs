using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

internal sealed class NewOsCommand  :ICommand
{
    public Result Execute(CommandContext ctx)
    {
        var deltabox = Path.Combine(ctx.Folder, Configure.DeltaBoxFile);
        if (!File.Exists(deltabox))
            return Error.DeltaBoxNotFound();

        var filesInDeltabox = File.ReadAllLines(deltabox);
        string format = string.Empty;
        foreach (var f in filesInDeltabox)
        {
            var info = f.Split('|');
            if (info[0].Equals("Directory"))
            {
                format = info[1];
            }
        }

        if (string.IsNullOrWhiteSpace(format))
            return new Error("Directory.NotFound", "Not Found Directory of files");
        var fileCurrent = Path.Combine(Environment.CurrentDirectory, format.Split(new[] { '\\', '/' }).Last());

        foreach (var f in filesInDeltabox)
        {
            var info = f.Split('|');
            if (!info[0].Equals("Branch") && info.Length == 4)
            {
                info[2] = Path.Combine(fileCurrent,info[2]);
            }
        }
        File.WriteAllLines(deltabox,filesInDeltabox);
        return Result.Success();
    }
}