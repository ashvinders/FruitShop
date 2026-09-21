using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FruitShop.Api.Tests;

public class BaseApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    public BaseApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}
