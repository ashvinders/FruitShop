using FruitShop.Domain.Products;

namespace FruitShop.Domain.Baskets;

public class FruitBasket
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public FruitStore Store { get; private set; } = null!;
    public decimal TotalPrice => _items.Sum(i => Math.Round(i.TotalPrice, 2));

    private readonly List<FruitBasketItem> _items = [];
    public IReadOnlyList<FruitBasketItem> Items => _items.AsReadOnly();

    private FruitBasket() { }

    public static FruitBasket New(FruitStore store)
    {
        Guard.Against.Null(store, nameof(store));
        return new FruitBasket { Store = store };
    }

    public void AddItem(Fruit fruit, decimal quantity, DateTime? createdOn = null)
    {
        Guard.Against.Null(fruit, nameof(fruit));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.Fruit.Name == fruit.Name);
           
        if (existingItem != null)  existingItem.ReviseQuantity(quantity);
        else _items.Add(FruitBasketItem.New(fruit, quantity, createdOn));
    }

    public void RemoveItem(Fruit fruit)
    {
        Guard.Against.Null(fruit, nameof(fruit));
        var existingItem = _items.FirstOrDefault(i => i.Fruit.Name == fruit.Name);
        if (existingItem != null) _items.RemoveAll(i => i.Fruit.Name == fruit.Name);
    }

    public void ClearItems()
    {
        _items.Clear();
    }    
}

