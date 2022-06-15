using System.Runtime.Serialization;

namespace forgedinthelore_net.Errors;

[Serializable]
public class SeedException : Exception
{
    public SeedException()
    {
    }

    public SeedException(string message) : base(message)
    {
    }

    public SeedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SeedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}