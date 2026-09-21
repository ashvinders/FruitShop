using FruitShop.Domain.Baskets;

namespace FruitShop.Domain.Pricing;

public abstract class PricingStrategy 
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public abstract decimal CalculatePrice(FruitBasketItem fruitItem, decimal itemTotal);
    protected PricingStrategy? NextStrategy  { get; private set; }
    public string Name => GetType().Name;

    public PricingStrategy ApplyNextStrategy(PricingStrategy nextStrategy)
    {
        Guard.Against.Null(nextStrategy, nameof(nextStrategy));
        NextStrategy = nextStrategy;
        return nextStrategy;
    }

}