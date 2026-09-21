namespace FruitShop.Domain.Guards;

public static class OutOfRangeExtension
{
    public static T OutOfRange<T>(this IGuardClause guardClause, T input,
        string parameterName,
        T rangeFrom,
        T rangeTo,
        string? message = null) where T : IComparable, IComparable<T>
    {
        if (rangeFrom.CompareTo(rangeTo) > 0)
        {
            throw new BadRequestException(message ?? $"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}");
        }

        if (input.CompareTo(rangeFrom) < 0 || input.CompareTo(rangeTo) > 0)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new BadRequestException($"{parameterName} should be between {rangeFrom} and {rangeTo}");
            }
            throw new BadRequestException(message);
        }

        return input;
    }
}
