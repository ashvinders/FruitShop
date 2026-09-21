using FruitShop.Domain.Pricing;

namespace FruitShop.Domain.Products;

public class Fruit
{
    public string Name { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    private readonly List<ProductPricingStrategy> _pricingStrategies = [];
    public IReadOnlyList<ProductPricingStrategy> PricingStrategies => _pricingStrategies.AsReadOnly();
    public PricingStrategy? PrimaryStrategy => _pricingStrategies.OrderBy(s => s.Priority).FirstOrDefault()?.PricingStrategy;

    public UnitOfMeasure UnitOfMeasure { get; private set; }

    private Fruit() { }

    public static Fruit New(string name, decimal basePrice, UnitOfMeasure unitOfMeasure)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.NegativeOrZero(basePrice, nameof(basePrice));

        var fruit = new Fruit
        {
            Name = name,
            BasePrice = basePrice,
            UnitOfMeasure = unitOfMeasure   
        };
        return fruit;
    }

    public void Rename(string newName)
    {
        Guard.Against.NullOrWhiteSpace(newName, nameof(newName));
        Name = newName;
    }

    public void AddPricingStrategy(PricingStrategy strategy)
    {
        Guard.Against.Null(strategy, nameof(strategy));
     
        if(_pricingStrategies.Any(p => p.PricingStrategy.GetType() == strategy.GetType()))
            throw new BadRequestException($"A pricing strategy of type {strategy.GetType().Name} already exists for this product.");

        var priority = _pricingStrategies.Count > 0 ? _pricingStrategies.Max(p => p.Priority) + 1 : 1;

        ProductPricingStrategy productPrice = ProductPricingStrategy.New(strategy, priority);
        _pricingStrategies.Add(productPrice);
        ApplyPricingStrategies();
    }

    public void ApplyPricingStrategies()
    {
        var orderedStrategies = _pricingStrategies.OrderBy(s => s.Priority).ToList();
        PricingStrategy? previousStrategy = null;

        foreach (var productPricingStrategy in orderedStrategies)
        {
            previousStrategy?.ApplyNextStrategy(productPricingStrategy.PricingStrategy);
            previousStrategy = productPricingStrategy.PricingStrategy;
        }
    }    

    public void ClearStrategies<T>() where T : PricingStrategy
    {
        _pricingStrategies.RemoveAll(p => p.PricingStrategy is T);
    }
}