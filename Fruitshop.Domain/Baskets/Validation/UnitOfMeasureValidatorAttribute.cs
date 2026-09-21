using FruitShop.Domain.Pricing;

namespace FruitShop.Domain.Baskets.Validation;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class UnitOfMeasureValidatorAttribute(UnitOfMeasure unitOfMeasure) : Attribute
{
    public UnitOfMeasure UnitOfMeasure { get; private set; } = unitOfMeasure;
}
