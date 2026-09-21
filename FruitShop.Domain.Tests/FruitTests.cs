namespace FruitShop.Domain.Tests;

public class FruitTests
{
    [Fact]
    public void It_Should_Create_A_Fruit_With_The_Given_Name_And_Price()
    {
        //arrange
        var price = 2.50m;
        var productName = "Apple";

        //act
        var product = Fruit.New(productName, price, UnitOfMeasure.PerItem);

        //assert
        Assert.Equal(productName, product.Name);
        Assert.Equal(price, product.BasePrice);
        Assert.Equal(UnitOfMeasure.PerItem, product.UnitOfMeasure);
    }

    [Fact]
    public void It_Should_Rename_A_Fruit()
    {
        //arrange
        var product = Fruit.New("Apple", 2.50m, UnitOfMeasure.PerItem);
        var newName = "Green Apple";

        //act
        product.Rename(newName);

        //assert
        Assert.Equal(newName, product.Name);
    }

    [Fact]
    public void AddPricingStrategy_NullStrategy_ThrowsBadRequestException()
    {
        var fruit = Fruit.New("Apple", 1.0m, UnitOfMeasure.PerItem);
        Assert.Throws<BadRequestException>(() => fruit.AddPricingStrategy(null!));
    }

    [Fact]
    public void AddPricingStrategy_DuplicateStrategy_ThrowsBadRequestException()
    {
        var fruit = Fruit.New("Apple", 1.0m, UnitOfMeasure.PerItem);

        var strat1 = BulkDiscountPricingStrategy.New(Discount.New(10, 20, 0.9m));
        fruit.AddPricingStrategy(strat1);

        var strat2 = BulkDiscountPricingStrategy.New(Discount.New(20, 40, 0.8m));
        Assert.Throws<BadRequestException>(() => fruit.AddPricingStrategy(strat2));
    }

    
}
