namespace DeltaBox.Commum;

public record Error(string Title, string Message)
{
    public static Error DeltaBoxNotFound() => new("DeltaBox.NotFound","DeltaBox File Not Found!");
    public static Error DirectoryNotFound() => new("Directory.NotFound","Directory Not Found!");
    public static Error CommandInvalid() => new("Command.Invalid","Command Invalid");

};