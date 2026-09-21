using System.Net;

namespace FruitShop.Domain.Exceptions;

public abstract class WebException(string message) : Exception(message)
{
    public abstract HttpStatusCode StatusCode { get; }
}
