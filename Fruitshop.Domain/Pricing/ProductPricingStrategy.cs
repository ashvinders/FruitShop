namespace FruitShop.Domain.Pricing;

public class ProductPricingStrategy
{
    public int Priority { get; private set; } = 1;
    public PricingStrategy PricingStrategy { get; private set; } = null!;

    private ProductPricingStrategy() { }

    public static ProductPricingStrategy New(PricingStrategy pricingStrategy, int priority)
    {
        Guard.Against.Null(pricingStrategy, nameof(pricingStrategy));
        Guard.Against.NegativeOrZero(priority, nameof(priority));

        return new ProductPricingStrategy
        {
            PricingStrategy = pricingStrategy,
            Priority = priority
        };
    }
}
