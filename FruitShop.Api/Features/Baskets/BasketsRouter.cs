using FruitShop.Api.Features.Baskets.Items;

namespace FruitShop.Api.Features.Baskets;

public static class BasketsRouter
{
    public static void MapBasketRoutes(this WebApplication app)
    {
        app.MapPost("api/baskets/", BasketActions.AddBasket);
        app.MapGet("api/baskets/{basketId:guid}", BasketActions.GetBasket);
        app.MapPut("api/baskets/{basketId:guid}/items", BasketItemsActions.AddBasketItems);
        app.MapPost("api/baskets/{basketId:guid}/items", BasketItemsActions.AddBasketItem);
    }
}
