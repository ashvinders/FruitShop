namespace FruitShop.Api.Features.Baskets.Items;

public static partial class BasketItemsActions
{
    public static async ValueTask<BasketSummary> AddBasketItems(Guid basketId, List<BasketItem> items, IStoreData storeData)
    {
        var basket = storeData.GetBasket(basketId);
        basket.ClearItems();

        foreach (var item in items)
            storeData.AddItemToBasket(basketId, item.FruitName, item.Quantity);

        var dtos = basket.Items.Select(i => new BasketItemDto(i.Fruit.Name, i.Quantity, i.Fruit.BasePrice, i.TotalPrice)).ToList();
        return new BasketSummary(basket.Id, dtos, basket.TotalPrice);
    }
}

public record BasketSummary(Guid Id, List<BasketItemDto> Items, decimal TotalPrice)
{

}