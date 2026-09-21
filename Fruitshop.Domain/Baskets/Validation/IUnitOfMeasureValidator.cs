using FruitShop.Domain.Products;

namespace FruitShop.Domain.Baskets.Validation;

internal interface IUnitOfMeasureValidator
{
    void ValidateQuantity(Fruit fruit, decimal quantity);
}
