using System.Net;

namespace FruitShop.Domain.Exceptions
{
    public class BadRequestException(string message) : WebException(message)
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
