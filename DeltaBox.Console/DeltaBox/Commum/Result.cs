namespace DeltaBox.Commum;

public class Result
{
    public bool IsSuccess { get;init; }
    public Error Error { get; init; }

    public Result()
    {
        IsSuccess = true;   
    }
    public Result(Error error)
    {
        IsSuccess = false;  
        Error = error;
    }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error)
        => Failure(error);
}