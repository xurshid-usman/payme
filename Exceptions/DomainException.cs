using Payme.Types;
namespace Payme.Exceptions;

public class DomainException : System.Exception
{
    protected DomainException(Error error)
    {
        Error = error;
    }

    public DomainException(string message, string data = "merchant")
    {
        Error = new Error(-31008, new Message(message, message, message), data);
    }
    
    public Error Error { get; set; }
}

