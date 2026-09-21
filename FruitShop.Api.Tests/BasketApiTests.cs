using FruitShop.Api.Features.Baskets.Items;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FruitShop.Api.Tests;

public class BasketApiTests(WebApplicationFactory<Program> factory) :  BaseApiTests(factory)
{
    [Fact]
    public async Task CreateBasket()
    {
        var basketItems = new List<BasketItem>
        {
            new BasketItem("Apple", 2),
            new BasketItem("Banana", 10),
            new BasketItem("Cherry", 12)
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/baskets", basketItems);
        response.EnsureSuccessStatusCode();
        var createdBasket = await response.Content.ReadFromJsonAsync<BasketSummary>();
        Assert.NotNull(createdBasket);
        Assert.Equal(3, createdBasket.Items.Count);
        Assert.Equal(37.48m, createdBasket.TotalPrice);
    }
}
