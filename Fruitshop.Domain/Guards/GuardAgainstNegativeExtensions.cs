namespace FruitShop.Domain.Guards;

public static partial class GuardClauseExtensions
{ 
    public static T Negative<T>(this IGuardClause guardClause,
        T input,
        string? parameterName = null,
        string? message = null) where T : struct, IComparable

    {
        if (input.CompareTo(default(T)) < 0)
            throw new BadRequestException(message ?? $"Required input {parameterName} cannot be negative.");

        return input;
    }

    public static int NegativeOrZero(this IGuardClause guardClause,
        int input,
        string? parameterName = null,
        string? message = null)

    {
        return NegativeOrZero<int>(guardClause, input, parameterName, message);
    }
    
    public static T NegativeOrZero<T>(this IGuardClause guardClause,
        T input,
        string? parameterName = null,
        string? message = null) where T : struct, IComparable
    {
        if (input.CompareTo(default(T)) <= 0)
            throw new BadRequestException(message ?? $"Required input {parameterName} cannot be zero or negative.");

        return input;
    }    
}
