namespace FruitShop.Api.Features.Baskets;

public static partial class BasketActions
{
    public static async ValueTask<BasketSummary> GetBasket(Guid basketId, IStoreData storeData)
    {
        FruitBasket basket =  storeData.GetBasket(basketId);
        var dtos = basket.Items.Select(i => new BasketItemDto(i.Fruit.Name, i.Quantity, i.Fruit.BasePrice, i.TotalPrice)).ToList();
        return new BasketSummary(basket.Id, dtos, basket.TotalPrice);
    }
}


public record BasketSummary(Guid Id, List<BasketItemDto> Items, decimal TotalPrice)
{
    
}

public record BasketItemDto(string FruitName, decimal Quantity, decimal BasePrice, decimal TotalPrice);