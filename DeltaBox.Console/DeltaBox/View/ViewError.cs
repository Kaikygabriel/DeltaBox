using DeltaBox.Commum;

namespace DeltaBox.View;

public class ViewError
{
    public static void Get(Error error)
    {
        Console.Error.WriteLine(error.Title);
        Console.Error.WriteLine(error.Message);
        Environment.Exit(1);
    }
}