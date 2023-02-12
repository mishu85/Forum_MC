namespace ApplicationCore.Exeptions;

public class NoRepliesToRepliesException : Exception
{
    public NoRepliesToRepliesException()
        : base($"No replies allowed to replies.")
    {
    }

    protected NoRepliesToRepliesException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }

    public NoRepliesToRepliesException(string message) : base(message)
    {
    }

    public NoRepliesToRepliesException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
