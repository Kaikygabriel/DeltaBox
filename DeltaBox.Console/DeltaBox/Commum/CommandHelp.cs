namespace DeltaBox.Commum;

public sealed record CommandHelp(
    string Usage,
    string ShortDescription,
    string Syntax,
    string[]? rules = null,
    string[]? examples = null)
{
    public string Name => Usage.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
    public string[]? Rules => rules;
    public string[]? Examples => examples;
}