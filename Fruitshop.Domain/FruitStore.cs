using FruitShop.Domain.Products;

namespace FruitShop.Domain;

public class FruitStore 
{
    public string Name { get; private set; } = null!;
    private readonly List<Fruit> _products = [];
    public IReadOnlyList<Fruit> Products => _products.AsReadOnly();

    private FruitStore() { }

    public static FruitStore New(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        return new FruitStore { Name = name };
    }
    public void AddProduct(Fruit product)
    {
        Guard.Against.Null(product, nameof(product));

        if(_products.Any(p => p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
            throw new BadRequestException($"Fruit with name '{product.Name}' already exists in the store.");

        _products.Add(product);
    }

    public void DiscontinueProduct(string productName)
    {
        Guard.Against.NullOrWhiteSpace(productName, nameof(productName));

        var product = _products.FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));        
        Guard.Against.NotFound(productName, product, "Fruit");
        
        _products.RemoveAll(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
    }
}
