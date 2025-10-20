public class OperationResult
{
    public bool Succeeded { get; private set; }
    public List<string> Errors { get; private set; } = new();

    public static OperationResult Success()
    {
        return new OperationResult { Succeeded = true };
    }

    public static OperationResult Failure(params string[] errors)
    {
        return new OperationResult
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}
