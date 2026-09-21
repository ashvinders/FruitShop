namespace FruitShop.Infrastructure;

public interface IStoreData
{
    IReadOnlyList<Fruit> AvailableFruits { get; }
    FruitStore Store { get; }
    Guid AddBasket();
    void AddDiscountStrategyToFruit(string fruitName, IList<Discount> discounts);
    void AddFruit(string name, decimal basePrice, UnitOfMeasure unitOfMeasure);
    void AddItemToBasket(Guid basketId, string fruitName, decimal quantity);
    void AddSeasonalStrategyToFruit(string fruitName, IList<SeasonalPrice> seasonalPrices);
    FruitBasket GetBasket(Guid basketId);
}

public class StoreData : IStoreData
{
    public IReadOnlyList<Fruit> AvailableFruits => Store.Products;
    private static readonly List<FruitBasket> FruitBaskets = [];
    private static readonly FruitStore _fruitStore = FruitStore.New("Acme");
    public FruitStore Store => _fruitStore;

    // Lock to prevent concurrent updates to the FruitBaskets collection and its baskets.
    private static readonly Lock _fruitBasketsLock = new();

    public static void SeedData()
    {
        lock (_fruitBasketsLock)
        {
            if (_fruitStore.Products.Any()) return;

            var apple = Fruit.New("Apple", 2.00m, UnitOfMeasure.Kilogram);
            _fruitStore.AddProduct(apple);

            var banana = Fruit.New("Banana", 0.3m, UnitOfMeasure.PerItem);
            _fruitStore.AddProduct(banana);
            var bulkBananaDiscount = Discount.New(10, decimal.MaxValue, 0.9m); // 10% discount for 10 or more items
            var bulkBananaDiscountStrategy = BulkDiscountPricingStrategy.New(bulkBananaDiscount);
            banana.AddPricingStrategy(bulkBananaDiscountStrategy);

            var cherry = Fruit.New("Cherry", 3.00m, UnitOfMeasure.Kilogram);
            _fruitStore.AddProduct(cherry);
            var bulkCherryDiscount = Discount.New(2, decimal.MaxValue, 0.95m); // 5% discount for 10 or more items
            var bulkCherryDiscountStrategy = BulkDiscountPricingStrategy.New(bulkCherryDiscount);
            cherry.AddPricingStrategy(bulkCherryDiscountStrategy);

            var summerCherryPrice = SeasonalPrice.New("Summer", 12, 1, 2, 28, 0.8m); // 20% discount during summer
            var autumnCherryPrice = SeasonalPrice.New("Autumn", 3, 2, 11, 30, 0.9m); // 10% discount during autumn
            var seasonalCherryStrategy = SeasonalPricingStrategy.New(summerCherryPrice);
            seasonalCherryStrategy.AddSeasonalPrice(autumnCherryPrice);
            cherry.AddPricingStrategy(seasonalCherryStrategy);
        }
    }

    public Guid AddBasket()
    {
        var basket = FruitBasket.New(Store);
        lock (_fruitBasketsLock)
        {
            FruitBaskets.Add(basket);
        }
        return basket.Id;
    }   

    public FruitBasket GetBasket(Guid basketId)
    {
        lock (_fruitBasketsLock)
        {
            var basket = FruitBaskets.FirstOrDefault(b =>   b.Id == basketId);
            Guard.Against.NotFound(basketId.ToString(), basket, "Basket");
            return basket!;
        }
    }

    public void AddItemToBasket(Guid basketId, string fruitName, decimal quantity)
    {
        lock (_fruitBasketsLock)
        {
            var basket = FruitBaskets.FirstOrDefault(b => b.Id == basketId);
            Guard.Against.NotFound(basketId.ToString(), basket, "Basket");

            var fruit = Store.Products.FirstOrDefault(f => f.Name.Equals(fruitName, StringComparison.OrdinalIgnoreCase));
            Guard.Against.NotFound(fruitName, fruit, "Product");

            basket!.AddItem(fruit!, quantity);
        }
    }

    public void AddFruit(string name, decimal basePrice, UnitOfMeasure unitOfMeasure)
    {
        var fruit = Fruit.New(name, basePrice, unitOfMeasure);

        lock (_fruitBasketsLock)
        {
            Store.AddProduct(fruit);
        }
    }

    public void AddDiscountStrategyToFruit(string fruitName, IList<Discount> discounts)
    {
        Guard.Against.NullOrWhiteSpace(fruitName, nameof(fruitName));
        Guard.Against.NullOrEmpty(discounts, nameof(discounts));


        var fruit = Store.Products.FirstOrDefault(f => f.Name.Equals(fruitName, StringComparison.OrdinalIgnoreCase));
        Guard.Against.NotFound(fruitName, fruit, "Product");

        fruit!.ClearStrategies<BulkDiscountPricingStrategy>();

        var bulkDiscountStrategy = BulkDiscountPricingStrategy.New(discounts[0]);

        for (int i = 1; i < discounts.Count; i++)
        {
            bulkDiscountStrategy.AddDiscount(discounts[i]);
        }

        fruit.AddPricingStrategy(bulkDiscountStrategy);

    }

    public void AddSeasonalStrategyToFruit(string fruitName, IList<SeasonalPrice> seasonalPrices)
    {
        Guard.Against.NullOrWhiteSpace(fruitName, nameof(fruitName));
        Guard.Against.NullOrEmpty(seasonalPrices, nameof(seasonalPrices));

        lock (_fruitBasketsLock)
        {
            var fruit = Store.Products.FirstOrDefault(f => f.Name.Equals(fruitName, StringComparison.OrdinalIgnoreCase));
            Guard.Against.NotFound(fruitName, fruit, "Product");

            fruit!.ClearStrategies<SeasonalPricingStrategy>();

            var seasonalStrategy = SeasonalPricingStrategy.New(seasonalPrices[0]);

            for (int i = 1; i < seasonalPrices.Count; i++)
            {
                seasonalStrategy.AddSeasonalPrice(seasonalPrices[i]);
            }

            fruit.AddPricingStrategy(seasonalStrategy);
        }
    }
}