using FruitShop.Domain;
using FruitShop.Domain.Exceptions;
using FruitShop.Domain.Pricing;
using FruitShop.Infrastructure;
using System;
using System.Linq;
using Xunit;

namespace FruitShop.Infrastructure.Tests;

public class StoreDataTests
{
    private StoreData _storeData;
    private StoreData GetStore()
    { 
        if(_storeData == null)
        {
            _storeData = new StoreData();
            StoreData.SeedData();
        }
        return _storeData;
    }

    [Fact]
    public void SeedData_Populates_Default_Fruits()
    {
        // Arrange / Act
        var storeData = GetStore();

        var productNames = storeData.AvailableFruits.Select(f => f.Name).ToList();

        // Assert
        Assert.Contains("Apple", productNames, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("Banana", productNames, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("Cherry", productNames, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddFruit_Adds_New_Product_To_Store()
    {
        // Arrange
        var storeData = GetStore();
        var uniqueName = "TestFruit_" + Guid.NewGuid().ToString("N");

        // Act
        storeData.AddFruit(uniqueName, 1.23m, UnitOfMeasure.Kilogram);

        // Assert
        var names = storeData.AvailableFruits.Select(f => f.Name);
        Assert.Contains(uniqueName, names);
    }

    [Fact]
    public void AddBasket_Then_GetBasket_Returns_Same_Id()
    {
        // Arrange
        var storeData = GetStore();

        // Act
        var id = storeData.AddBasket();
        var basket = storeData.GetBasket(id);

        // Assert
        Assert.Equal(id, basket.Id);
    }

    [Fact]
    public void AddItemToBasket_With_Invalid_Basket_Throws()
    {
        // Arrange
        var storeData = GetStore();
        var uniqueFruit = "FruitForInvalidBasket_" + Guid.NewGuid().ToString("N");
        storeData.AddFruit(uniqueFruit, 0.5m, UnitOfMeasure.PerItem);

        // Act & Assert
        Assert.ThrowsAny<NotFoundException>(() => storeData.AddItemToBasket(Guid.NewGuid(), uniqueFruit, 1m));
    }

    [Fact]
    public void AddItemToBasket_With_Invalid_Product_Throws()
    {
        // Arrange
        var storeData = GetStore();
        var basketId = storeData.AddBasket();

        // Act & Assert
        Assert.ThrowsAny<NotFoundException>(() => storeData.AddItemToBasket(basketId, "NonExistingProduct_" + Guid.NewGuid().ToString("N"), 1m));
    }
}