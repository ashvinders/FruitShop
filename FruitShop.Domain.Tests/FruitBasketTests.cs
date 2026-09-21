namespace FruitShop.Domain.Tests.Baskets;

public class FruitBasketTests
{
    [Fact]
    public void New_NullStore_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() => FruitBasket.New(null!));
    }

    [Fact]
    public void New_WithStore_SetsStoreAndEmptyItems()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);

        Assert.Equal(store, basket.Store);
        Assert.Empty(basket.Items);
        Assert.Equal(0m, basket.TotalPrice);
    }

    [Fact]
    public void AddItem_NullFruit_ThrowsBadRequestException()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);

        Assert.Throws<BadRequestException>(() => basket.AddItem(null!, 1m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_NonPositiveQuantity_ThrowsBadRequestException(decimal qty)
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        var fruit = Fruit.New("Apple", 1m, UnitOfMeasure.PerItem);

        Assert.Throws<BadRequestException>(() => basket.AddItem(fruit, qty));
    }

    [Fact]
    public void AddItem_NewItem_AddsToItemsAndUpdatesTotalPrice()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        var apple = Fruit.New("Apple", 2.0m, UnitOfMeasure.PerItem);

        basket.AddItem(apple, 3m);

        Assert.Single(basket.Items);
        Assert.Equal(3m, basket.Items[0].Quantity);
        Assert.Equal(6.0m, basket.TotalPrice);
    }

    [Fact]
    public void AddItem_ExistingFruit_RevisesQuantity_NotAddsDuplicate()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        var apple = Fruit.New("Apple", 2.0m, default);

        basket.AddItem(apple, 2m);
        // add another Fruit instance with same name - basket identifies by name
        basket.AddItem(Fruit.New("Apple", 2.0m, default), 5m);

        Assert.Single(basket.Items);
        Assert.Equal(5m, basket.Items[0].Quantity);
    }

    [Fact]
    public void RemoveItem_NullFruit_ThrowsBadRequestException()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);

        Assert.Throws<BadRequestException>(() => basket.RemoveItem(null!));
    }

    [Fact]
    public void RemoveItem_RemovesItemFromBasket()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        var apple = Fruit.New("Apple", 1m, default);
        basket.AddItem(apple, 1m);

        basket.RemoveItem(apple);

        Assert.Empty(basket.Items);
    }

    [Fact]
    public void ClearBasket_RemovesAllItemsAndResetsTotal()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        basket.AddItem(Fruit.New("A", 1m, default), 2m);
        basket.AddItem(Fruit.New("B", 3m, default), 1m);

        basket.ClearItems();

        Assert.Empty(basket.Items);
        Assert.Equal(0m, basket.TotalPrice);
    }

    [Fact]
    public void AddMultipleItems_CalculatesTotalPriceCorrectly()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);
        var apple = Fruit.New("Apple", 2.0m, UnitOfMeasure.PerItem);
        var banana = Fruit.New("Banana", 1.5m, UnitOfMeasure.Kilogram);
        basket.AddItem(apple, 3m); // 6.0
        basket.AddItem(banana, 4.5m); // 6.75
        Assert.Equal(12.75m, basket.TotalPrice);
    }

    [Fact]
    public void AddMultipleItems_WithMultipleStrategies()
    {
        var store = FruitStore.New("Store");
        var basket = FruitBasket.New(store);

        var apple = Fruit.New("Apple", 2.0m, UnitOfMeasure.PerItem);
        var discount = Discount.New(3, decimal.MaxValue, 0.9m); // 10% off for 3 or more
        var discountStrategy = BulkDiscountPricingStrategy.New(discount); 
        apple.AddPricingStrategy(discountStrategy);

        var banana = Fruit.New("Banana", 3m, UnitOfMeasure.Kilogram);
        
        var seasonPrice = SeasonalPrice.New("Summer", 6, 1, 8, 31, 0.8m); // 20% off from June 1 to Aug 31
        var seasonalStrategy = SeasonalPricingStrategy.New(seasonPrice);
        banana.AddPricingStrategy(seasonalStrategy);

        var bananaBulkDiscount = Discount.New(5, decimal.MaxValue, 0.85m); // 15% off for 5kg or more
        var bananaBulkDiscountStrategy = BulkDiscountPricingStrategy.New(bananaBulkDiscount);
        banana.AddPricingStrategy(bananaBulkDiscountStrategy);

        var priceDate = new DateTime(DateTime.Now.Year, 7, 15); // July 15

        basket.AddItem(apple, 4m, priceDate); // 4 apples, should apply discount
        basket.AddItem(banana, 7m, priceDate); // 7 kg bananas, should apply seasonal price and bulk discount

        Assert.Equal(7.2m + 14.28m, basket.TotalPrice); // 7.2 + 14.28 = 21.48

    }
}