using DeltaBox.Commum;

namespace DeltaBox.View;

public class ViewError
{
    public static void Get(Error error)
    {
        Console.WriteLine(error.Title);
        Console.WriteLine(error.Message);
    }
}