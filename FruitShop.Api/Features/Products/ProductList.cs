namespace FruitShop.Api.Features.Products;

public static partial class ProductActions
{
    public static async ValueTask<IList<Fruit>> GetProducts(IStoreData storeData)
    {
        return [.. storeData.AvailableFruits];
    }
}