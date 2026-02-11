using DeltaBox.Commum;

namespace DeltaBox.Abstraction;

public interface ICommand
{
    Result Execute(CommandContext ctx);
}