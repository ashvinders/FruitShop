namespace FruitShop.Domain.Guards;

public interface IGuardClause
{
}

public class Guard : IGuardClause
{
    public static IGuardClause Against { get; } = new Guard();
    public Guard() { }
}
