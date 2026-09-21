using System.Diagnostics.CodeAnalysis;
namespace FruitShop.Domain.Guards;

public static partial class GuardClauseExtensions
{
    public static T Null<T>(this IGuardClause guardClause, T input,
        string? parameterName = null,
        string? message = null)
    {
        if (input is null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new BadRequestException(message ?? $"Required input {parameterName} was empty.");
            }
            throw new BadRequestException(message);
        }

        return input;
    }

    public static string NullOrEmpty(this IGuardClause guardClause, string input,
        string? parameterName = null,
        string? message = null)
    {
        Guard.Against.Null(input, parameterName, message);
        if (input == string.Empty)
        {
            throw new BadRequestException(message ?? $"Required input {parameterName} was empty.");
        }

        return input;
    }

    public static Guid NullOrEmpty(this IGuardClause guardClause,
        Guid? input,
        string? parameterName = null,
        string? message = null)
    {
        Guard.Against.Null(input, parameterName, message);
        if (input == Guid.Empty)
        {
            throw new BadRequestException(message ?? $"Required input {parameterName} was empty.");
        }

        return input!.Value;
    }

    public static IEnumerable<T> NullOrEmpty<T>(this IGuardClause guardClause,
        IEnumerable<T> input,
        string? parameterName = null,
        string? message = null)
    {
        Guard.Against.Null(input, parameterName, message);
        if (!input.Any())
        {
            throw new BadRequestException(message ?? $"Required input {parameterName} was empty.");
        }

        return input;
    }

    public static string NullOrWhiteSpace(this IGuardClause guardClause,
        string input,
        string? parameterName = null,
        string? message = null)
    {
        Guard.Against.NullOrEmpty(input, parameterName, message);
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new BadRequestException(message ?? $"Required input {parameterName} was empty.");
        }

        return input;
    }

    public static T Default<T>(this IGuardClause guardClause,
        [AllowNull, NotNull] T input,
        string? parameterName = null,
        string? message = null)

    {
        if (EqualityComparer<T>.Default.Equals(input, default(T)!) || input is null)
        {
            throw new BadRequestException(message ?? $"Parameter [{parameterName}] is default value for type {typeof(T).Name}");
        }

        return input;
    }

}
