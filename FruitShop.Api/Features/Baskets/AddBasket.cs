using FruitShop.Api.Features.Baskets.Items;

namespace FruitShop.Api.Features.Baskets;

public static partial class BasketActions
{
    public static async ValueTask<BasketSummary> AddBasket(List<BasketItem> basketItems, IStoreData storeData)
    {       
        var id = storeData.AddBasket();

        if(basketItems is not null)
        {
            foreach (var item in basketItems)
            {
                storeData.AddItemToBasket(id, item.FruitName, item.Quantity);
            }
        }

        FruitBasket basket = storeData.GetBasket(id);
        var dtos = basket.Items.Select(i => new BasketItemDto(i.Fruit.Name, i.Quantity, i.Fruit.BasePrice, i.TotalPrice)).ToList();
        return new BasketSummary(basket.Id, dtos, basket.TotalPrice);
    }
}
