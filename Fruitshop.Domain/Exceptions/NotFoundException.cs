using System.Net;

namespace FruitShop.Domain.Exceptions;

public class NotFoundException : WebException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public NotFoundException(string resourceType, string key, string value)
        : base($"{resourceType} with {key} '{value}' was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
