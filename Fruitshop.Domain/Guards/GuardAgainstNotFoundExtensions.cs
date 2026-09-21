namespace FruitShop.Domain.Guards;

public static class GuardAgainstNotFoundExtensions
{
    public static T NotFound<T>(this IGuardClause guardClause,
       string key,
       T input,
       string keyName)
    {
        guardClause.NullOrEmpty(key, nameof(key));

        if (input is null)
            throw new NotFoundException($"{keyName} {key} was not found");

        return input;
    }
}
