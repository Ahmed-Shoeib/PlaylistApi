namespace PlaylistApi.Exceptions;

/// <summary>
/// Thrown by the service layer when a requested resource does not exist.
/// Caught by global exception-handling middleware and translated to HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}