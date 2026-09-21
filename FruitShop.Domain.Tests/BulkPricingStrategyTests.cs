using FruitShop.Domain.Baskets;

namespace FruitShop.Domain.Tests;

public class BulkDiscountPricingStrategyTests
{

    [Fact]
    public void New_WithDiscount_AddsDiscount()
    {
        var discount = Discount.New(1m, 4m, 0.9m);
        var strategy = BulkDiscountPricingStrategy.New(discount);

        Assert.Single(strategy.Discounts);
        Assert.Equal(discount, strategy.Discounts[0]);
    }

    [Fact]
    public void AddDiscount_Overlapping_ThrowsBadRequestException()
    {
        var strategy = BulkDiscountPricingStrategy.New(Discount.New(1m, 5m, 0.9m));
        var overlapping = Discount.New(4m, 10m, 0.8m);

        Assert.Throws<BadRequestException>(() => strategy.AddDiscount(overlapping));
    }

    [Fact]
    public void AddDiscount_NonOverlapping_AllowsAddition()
    {
        var strategy = BulkDiscountPricingStrategy.New(Discount.New(1m, 4m, 0.9m));
        var next = Discount.New(5m, 10m, 0.8m);

        strategy.AddDiscount(next);

        Assert.Equal(2, strategy.Discounts.Count);
    }

    [Fact]
    public void CalculatePrice_WhenQuantityWithinRange_AppliesMultiplier()
    {
        // Use default UnitOfMeasure to avoid coupling to a specific enum value in tests
        var fruit = Fruit.New("Apple", 2.00m, default);
        var item = FruitBasketItem.New(fruit, 5m);
        var discount = Discount.New(5m, 10m, 0.8m);
        var strategy = BulkDiscountPricingStrategy.New(discount);

        var price = strategy.CalculatePrice(item, item.BaseTotal);

        // 5 * 2.00 = 10.00 -> 10.00 * 0.8 = 8.00 (rounded to 2 decimals)
        Assert.Equal(8.00m, price);
    }

    [Fact]
    public void CalculatePrice_WhenNoMatchingDiscount_ReturnsOriginal()
    {
        var fruit = Fruit.New("Apple", 2.00m, default);
        var item = FruitBasketItem.New(fruit, 3m);
        var discount = Discount.New(5m, 10m, 0.8m);
        var strategy = BulkDiscountPricingStrategy.New(discount);

        var price = strategy.CalculatePrice(item, item.BaseTotal);

        Assert.Equal(item.BaseTotal, price);
    }

    [Fact]
    public void AddDiscount_Null_ThrowsBadRequestException()
    {
        var strategy = BulkDiscountPricingStrategy.New(Discount.New(1m, 4m, 0.9m));
        Assert.Throws<BadRequestException>(() => strategy.AddDiscount(null!));
    }

}