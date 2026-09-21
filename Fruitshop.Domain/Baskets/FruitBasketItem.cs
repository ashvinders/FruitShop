using FruitShop.Domain.Baskets.Validation;
using FruitShop.Domain.Products;

namespace FruitShop.Domain.Baskets;

public class FruitBasketItem
{
    public Fruit Fruit { get; private set; } = null!;
    public decimal Quantity { get; private set; } = 1m;
    public DateTime CreatedOn { get; private set; }
    public decimal BaseTotal => Fruit.BasePrice * Quantity;
    public decimal TotalPrice => Fruit.PrimaryStrategy?.CalculatePrice(this, BaseTotal) ?? BaseTotal;

    private FruitBasketItem() { }

    public static FruitBasketItem New(Fruit fruit, decimal quantity, DateTime? createdOn = null)
    {
        Guard.Against.Null(fruit, nameof(fruit));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        ValidateQty(fruit, quantity);

        return new FruitBasketItem
        {
            Fruit = fruit,
            Quantity = quantity,
            CreatedOn = createdOn ?? DateTime.UtcNow

        };
    }

    public void ReviseQuantity(decimal newQuantity)
    {
        Guard.Against.NegativeOrZero(newQuantity, nameof(newQuantity));
        ValidateQty(Fruit, newQuantity);
        Quantity = newQuantity;
    }

    private static void ValidateQty(Fruit fruit, decimal quantity)
    {
        var quantityValidators = UnitOfMeasureValidatorFactory.GetValidators(fruit);

        foreach (var validator in quantityValidators)
        {
            validator.ValidateQuantity(fruit, quantity);
        }
    }    
}
