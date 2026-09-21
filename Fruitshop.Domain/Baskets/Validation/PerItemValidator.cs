using FruitShop.Domain.Pricing;
using FruitShop.Domain.Products;

namespace FruitShop.Domain.Baskets.Validation;

[UnitOfMeasureValidator(UnitOfMeasure.PerItem)]
internal class PerItemValidator : IUnitOfMeasureValidator
{
    public void ValidateQuantity(Fruit fruit, decimal quantity)
    {
        if(!decimal.IsInteger(quantity))
            throw new BadRequestException($"Quantity must be a whole number for {fruit.Name}s.");
    }
}
