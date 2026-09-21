namespace FruitShop.Api.Features.Products;

public static class ProductRouter
{
    public static void MapProductRoutes(this WebApplication app)
    {
        app.MapGet("api/products", ProductActions.GetProducts);
        app.MapPost("api/products", ProductActions.AddProduct);
    }
}
