using FruitShop.Api.Tests;
using FruitShop.Domain.Products;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class ProductApiTests(WebApplicationFactory<Program> factory) : BaseApiTests(factory)
{
    [Fact]
    public async Task GetProducts_ReturnsSuccessAndJson()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<FruitItem>? fruitItems = await response.Content.ReadFromJsonAsync<List<FruitItem>>();
        Assert.NotNull(fruitItems);
        Assert.NotEmpty(fruitItems);
    }

    [Fact]
    public async Task AddProduct_ReturnsSuccess()
    {
        var newFruit = new FruitItem("Mango", 1.5m, UnitOfMeasure.PerItem.ToString());
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/products", newFruit);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Verify that the product was added
        List<FruitItem>? fruitItems = await _client.GetFromJsonAsync<List<FruitItem>>("/api/products");
        Assert.NotNull(fruitItems);
        Assert.Contains(fruitItems, f => f.Name == "Mango" && f.BasePrice == 1.5m && f.UnitOfMeasure == UnitOfMeasure.PerItem.ToString());
    }

    private record FruitItem(string Name, decimal BasePrice, string UnitOfMeasure);
}
