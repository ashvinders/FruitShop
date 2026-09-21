namespace FruitShop.Api.Features.Products;

public static partial class ProductActions
{
    public static async ValueTask AddProduct(AddFruitModel fruit, IStoreData storeData)
    {
        bool validUOM = Enum.TryParse<UnitOfMeasure>(fruit.UnitOfMeasure, true, out var unitOfMeasure);

        if(!validUOM)
            throw new BadRequestException($"Invalid unit of measure: {fruit.UnitOfMeasure}");

        storeData.Store.AddProduct( Fruit.New( fruit.Name, fruit.BasePrice, unitOfMeasure ));
    }
}

public record AddFruitModel(string Name, decimal BasePrice, string UnitOfMeasure);