using FruitShop.Domain.Baskets;

namespace FruitShop.Domain.Pricing;

public class Discount
{
    public Guid Id { get; private set; }
    public decimal MinimumQty { get; private set; }
    public decimal MaximumQty { get; private set; }
    public decimal PriceMultiplier { get; private set; }

    private Discount() { }

    public static Discount New(decimal minimumQty, decimal maximumQty, decimal priceMultiplier)
    {
        Guard.Against.NegativeOrZero(minimumQty, nameof(minimumQty));
        Guard.Against.NegativeOrZero(maximumQty, nameof(maximumQty));
        Guard.Against.Negative(priceMultiplier, nameof(priceMultiplier));

        return new Discount
        {
            Id = Guid.NewGuid(),
            MinimumQty = minimumQty,
            MaximumQty = maximumQty,
            PriceMultiplier = priceMultiplier
        };
    }
}

public class BulkDiscountPricingStrategy : PricingStrategy
{
    private readonly List<Discount> _discounts = [];
    public IReadOnlyList<Discount> Discounts => _discounts.AsReadOnly();

    private BulkDiscountPricingStrategy() { }

    public static BulkDiscountPricingStrategy New(Discount discount)
    {
        var strategy = new BulkDiscountPricingStrategy();
        strategy.AddDiscount(discount);
        return strategy;
    }

    public void AddDiscount(Discount discount)
    {
        Guard.Against.Null(discount, nameof(discount));
        CheckOverlaps(discount);

        _discounts.Add(discount);
    }

    private void CheckOverlaps(Discount discount)
    {
        // check for overlapping ranges
        var overlaps = _discounts.Any(d =>
            (discount.MinimumQty <= d.MaximumQty && discount.MaximumQty >= d.MinimumQty)
            || (discount.MinimumQty <= d.MinimumQty && discount.MaximumQty >= d.MinimumQty && discount.MaximumQty <= d.MaximumQty)
            || (discount.MinimumQty >= d.MinimumQty && discount.MinimumQty <= d.MaximumQty && discount.MaximumQty >= d.MaximumQty)
            );

        if (overlaps) throw new BadRequestException("Overlapping discounts are not allowed.");
    }

    public void RemoveDiscount(Guid discountId)
    {
        Guard.Against.Null(discountId, nameof(discountId));

        if(!_discounts.Any(d => d.Id == discountId)) 
            throw new NotFoundException($"Discount with ID '{discountId}' was not found.");

        if(_discounts.Count == 1)
            throw new BadRequestException("Discount strategy must have at least one discount.");

        _discounts.RemoveAll(d => d.Id == discountId);
    }

    public void AdjustDiscount(Guid discountId, Discount updatedDiscount)
    {
        Guard.Against.Null(discountId, nameof(discountId));
        Guard.Against.Null(updatedDiscount, nameof(updatedDiscount));

        var existingDiscount = _discounts.FirstOrDefault(d => d.Id == discountId) ?? throw new BadRequestException($"Discount with ID '{discountId}' does not exist.");
        
        // Temporarily remove the existing discount to check for overlaps
        _discounts.Remove(existingDiscount);
        CheckOverlaps(updatedDiscount);

        _discounts.Add(updatedDiscount);
    }

    public override decimal CalculatePrice(FruitBasketItem fruitItem, decimal itemTotal)
    {
        Guard.Against.Null(fruitItem, nameof(fruitItem));
        Guard.Against.NegativeOrZero(itemTotal, nameof(itemTotal));

        decimal price = itemTotal;

        var applicableDiscount = _discounts.FirstOrDefault(x => fruitItem.Quantity >= x.MinimumQty && fruitItem.Quantity <= x.MaximumQty);

        if (applicableDiscount is not null) price = Math.Round(itemTotal * applicableDiscount.PriceMultiplier, 2);

        return NextStrategy?.CalculatePrice(fruitItem, price) ?? price;
    }

}
