using DeltaBox.Abstraction;
using DeltaBox.Commum;

namespace DeltaBox.Commands;

internal sealed class PoolCommand : ICommand
{
    public Result Execute(CommandContext ctx)
    {
        return Result.Success();
    }
}