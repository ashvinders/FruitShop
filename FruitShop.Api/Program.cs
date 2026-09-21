using FruitShop.Api.Features.Baskets;
using FruitShop.Api.Features.Products;
using FruitShop.Api.Middleware;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using FruitShop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.RegisterCoreServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!builder.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapProductRoutes();
app.MapBasketRoutes();
app.UseHttpsRedirection();

StoreData.SeedData();
app.Run();
