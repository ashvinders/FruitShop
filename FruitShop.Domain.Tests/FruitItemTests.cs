namespace FruitShop.Domain.Tests.Baskets;

public class FruitItemTests
{
    [Fact]
    public void New_WithValidFruitAndQuantity_SetsPropertiesAndTotals()
    {
        var fruit = Fruit.New("Apple", 2.00m, default);
        var item = FruitBasketItem.New(fruit, 3m);

        Assert.Equal(fruit, item.Fruit);
        Assert.Equal(3m, item.Quantity);
        Assert.Equal(6.00m, item.BaseTotal);
        Assert.Equal(item.BaseTotal, item.TotalPrice);
    }

    [Fact]
    public void New_NullFruit_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() => FruitBasketItem.New(null!, 1m));
    }

    [Fact]
    public void New_ZeroQuantity_ThrowsBadRequestException()
    {
        var fruit = Fruit.New("Apple", 1.00m, default);
        Assert.Throws<BadRequestException>(() => FruitBasketItem.New(fruit, 0m));
    }

    [Fact]
    public void New_PerItem_FractionalQuantity_ThrowsBadRequestException()
    {
        var fruit = Fruit.New("Apple", 1.00m, UnitOfMeasure.PerItem);
        Assert.Throws<BadRequestException>(() => FruitBasketItem.New(fruit, 1.5m));
    }

    [Fact]
    public void ReviseQuantity_Valid_UpdatesQuantityAndBaseTotal()
    {
        var fruit = Fruit.New("Apple", 2.00m, default);
        var item = FruitBasketItem.New(fruit, 2m);

        item.ReviseQuantity(5m);

        Assert.Equal(5m, item.Quantity);
        Assert.Equal(10.00m, item.BaseTotal);
    }

    [Fact]
    public void ReviseQuantity_PerItemFractional_ThrowsBadRequestException()
    {
        var fruit = Fruit.New("Apple", 2.00m, UnitOfMeasure.PerItem);
        var item = FruitBasketItem.New(fruit, 2m);

        Assert.Throws<BadRequestException>(() => item.ReviseQuantity(2.5m));
    }

    [Fact]
    public void TotalPrice_WithPrimaryStrategy_AppliesPricingStrategy()
    {
        var fruit = Fruit.New("Apple", 2.00m, UnitOfMeasure.Kilogram);
        var discount = Discount.New(2.5m, 10m, 0.5m);
        var strategy = BulkDiscountPricingStrategy.New(discount);
        fruit.AddPricingStrategy(strategy);

        var item = FruitBasketItem.New(fruit, 3.5m);

        // BaseTotal = 2 * 3.50 = 7.00, with multiplier 0.5 => 3.50
        Assert.Equal(3.50m, item.TotalPrice);
    }

    [Fact]
    public void TotalPrice_WithPrimaryStrategy_AppliesMulitplePricingStrategies()
    {
        var fruit = Fruit.New("Apple", 2.00m, default);
        var discount = Discount.New(1m, 10m, 0.5m);
        var discountStrategy = BulkDiscountPricingStrategy.New(discount);
        fruit.AddPricingStrategy(discountStrategy);
        // BaseTotal = 2 * 2.00 = 4.00, with multiplier 0.5 => 2.00

        var seasonPrice = SeasonalPrice.New("All Year", 1, 1, 12, 31, 0.5m);
        var seasonalStrategy = SeasonalPricingStrategy.New(seasonPrice);
        fruit.AddPricingStrategy(seasonalStrategy);
        // 2.00, with multiplier 0.5 => 1.00
        
        var item = FruitBasketItem.New(fruit, 2m);

        Assert.Equal(1.00m, item.TotalPrice);
    }
    
    [Fact]
    public void MultiplePricingStrategies_SeasonalPriceNotInSeason_AppliesOnlyBulkDiscount()
    {
        var fruit = Fruit.New("Apple", 2.00m, UnitOfMeasure.Liter);
        var discount = Discount.New(1m, 10m, 0.5m);
        var discountStrategy = BulkDiscountPricingStrategy.New(discount);
        fruit.AddPricingStrategy(discountStrategy);
        // BaseTotal = 1.5 * 2.00 = 3.00, with multiplier 0.5 => 1.50
        var seasonPrice = SeasonalPrice.New("Winter", 12, 1, 12, 31, 0.5m);
        var seasonalStrategy = SeasonalPricingStrategy.New(seasonPrice);
        fruit.AddPricingStrategy(seasonalStrategy);
        // Not in season, so no multiplier applied
        var item = FruitBasketItem.New(fruit, 1.5m, new DateTime(2001, 1, 1));
        Assert.Equal(1.50m, item.TotalPrice);
    }
}