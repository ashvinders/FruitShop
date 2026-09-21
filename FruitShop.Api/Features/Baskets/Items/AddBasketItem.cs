namespace FruitShop.Api.Features.Baskets.Items;

public static partial class BasketItemsActions
{   
    public static async ValueTask AddBasketItem(Guid basketId, BasketItem item, IStoreData storeData)
    {       
        storeData.AddItemToBasket(basketId, item.FruitName, item.Quantity);           
    }
}

