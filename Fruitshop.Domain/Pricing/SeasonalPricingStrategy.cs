using FruitShop.Domain.Baskets;

namespace FruitShop.Domain.Pricing;

public class SeasonalPrice
{
    public string SeasonName { get; private set; } = null!;
    public int StartingMonth { get; private set; } = 1;
    public int StartingDay { get; private set; } = 1;
    public int EndingMonth { get; private set; } = 12;
    public int EndingDay { get; private set; } = 31;
    public decimal PriceMultiplier { get; private set; } = 1.0m;

    private SeasonalPrice() { }

    public static SeasonalPrice New(string seasonName, int startingMonth, int startingDay, int endingMonth, int endingDay, decimal priceMultiplier)
    {
        Guard.Against.NullOrEmpty(seasonName, nameof(seasonName));
        Guard.Against.NegativeOrZero(startingMonth, nameof(startingMonth));
        Guard.Against.OutOfRange(startingMonth, nameof(startingMonth), 1, 12);
        Guard.Against.NegativeOrZero(startingDay, nameof(startingDay));
        Guard.Against.OutOfRange(startingDay, nameof(startingDay), 1, 31);
        Guard.Against.NegativeOrZero(endingMonth, nameof(endingMonth));
        Guard.Against.OutOfRange(endingMonth, nameof(endingMonth), 1, 12);
        Guard.Against.NegativeOrZero(endingDay, nameof(endingDay));
        Guard.Against.OutOfRange(endingDay, nameof(endingDay), 1, 31);
        Guard.Against.NegativeOrZero(priceMultiplier, nameof(priceMultiplier));
        
        if (startingMonth == 2 && startingDay == 29)
            throw new BadRequestException("Starting day cannot be February 29th, as this is not a valid date in non-leap years.");
        if (endingMonth == 2 && endingDay == 29)
            throw new BadRequestException("Ending day cannot be February 29th, as this is not a valid date in non-leap years.");

        return new SeasonalPrice
        {
            SeasonName = seasonName,
            StartingMonth = startingMonth,
            StartingDay = startingDay,
            EndingMonth = endingMonth,
            EndingDay = endingDay,
            PriceMultiplier = priceMultiplier
        };
    }

    public bool IsInSeason(DateTime pricedOn)
    {
        DateTime startingFrom = new(pricedOn.Year, StartingMonth, StartingDay);
        int endingYear = startingFrom.Year;

        if (EndingMonth < StartingMonth || (EndingMonth == StartingMonth && EndingDay < StartingDay))
            endingYear++;

        DateTime endingOn = new(endingYear, EndingMonth, EndingDay);

        return pricedOn >= startingFrom && pricedOn <= endingOn;
    }
}

public class SeasonalPricingStrategy : PricingStrategy
{
    private readonly List<SeasonalPrice> _prices = [];
    public IReadOnlyList<SeasonalPrice> Prices => _prices.AsReadOnly();

    private SeasonalPricingStrategy() { }
    public static SeasonalPricingStrategy New(SeasonalPrice seasonalPrice, DateTime? applicableDate = null)
    {
        var strategy = new SeasonalPricingStrategy();
        strategy.AddSeasonalPrice(seasonalPrice);
        return strategy;
    }

    public void AddSeasonalPrice(SeasonalPrice seasonalPrice)
    {
        Guard.Against.Null(seasonalPrice, nameof(seasonalPrice));     
        
        if(_prices.Any(p => p.SeasonName == seasonalPrice.SeasonName))
            throw new BadRequestException($"Season '{seasonalPrice.SeasonName}' already exists.");

        CheckOverlaps(seasonalPrice);
        _prices.Add(seasonalPrice);
    }

    public void RemoveSeasonalPrice(string seasonName)
    {
        Guard.Against.NullOrEmpty(seasonName, nameof(seasonName));

        if(!_prices.Any(p => p.SeasonName == seasonName))
            throw new NotFoundException($"Season '{seasonName}' was not found.");

        if (_prices.Count == 1)
            throw new BadRequestException("Seasonal strategy must have at least one seasonal price.");

        _prices.RemoveAll(p => p.SeasonName == seasonName);
    }

    public void AdjustSeasonalPrice(string seasonName, SeasonalPrice updatedPrice)
    {
        Guard.Against.NullOrEmpty(seasonName, nameof(seasonName));
        Guard.Against.Null(updatedPrice, nameof(updatedPrice));
        var existingPrice = _prices.FirstOrDefault(p => p.SeasonName == seasonName) ?? throw new BadRequestException($"Season '{seasonName}' does not exist.");
        
        // Temporarily remove the existing price to check for overlaps
        _prices.Remove(existingPrice);
        CheckOverlaps(updatedPrice);
        
        _prices.Add(updatedPrice);
    }

    override public decimal CalculatePrice(FruitBasketItem fruitItem, decimal itemTotal)
    {
        Guard.Against.Null(fruitItem, nameof(fruitItem));

        var seasonalPrice = _prices.FirstOrDefault(p => p.IsInSeason(fruitItem.CreatedOn));

        if (seasonalPrice != null)
            itemTotal *= seasonalPrice.PriceMultiplier;

        return NextStrategy?.CalculatePrice(fruitItem, itemTotal) ?? itemTotal;
    }

    private void CheckOverlaps(SeasonalPrice price)
    {

        Guard.Against.Null(price, nameof(price));

        // Use a non-leap reference year so DayOfYear is stable
        const int referenceYear = 2001;
        int thisStart = new DateTime(referenceYear, price.StartingMonth, price.StartingDay).DayOfYear;
        int thisEnd = new DateTime(referenceYear, price.EndingMonth, price.EndingDay).DayOfYear;

        IEnumerable<(int start, int end)> thisIntervals = ExpandIntervals(thisStart, thisEnd);


        foreach (SeasonalPrice existingPrice in _prices)
        {           
            // compute other's intervals
            int otherStart = new DateTime(referenceYear, existingPrice.StartingMonth, existingPrice.StartingDay).DayOfYear;
            int otherEnd = new DateTime(referenceYear, existingPrice.EndingMonth, existingPrice.EndingDay).DayOfYear;
            IEnumerable<(int start, int end)> otherIntervals = ExpandIntervals(otherStart, otherEnd);

            // check any interval intersection
            foreach (var ti in thisIntervals)
            {
                foreach ((int start, int end) in otherIntervals)
                {
                    int overlapStart = Math.Max(ti.start, start);
                    int overlapEnd = Math.Min(ti.end, end);
                    if (overlapStart <= overlapEnd)
                    {
                        throw new BadRequestException($"Season '{price.SeasonName}' overlaps with existing season '{existingPrice.SeasonName}'.");
                    }
                }
            }
        }
    }

    private static IEnumerable<(int start, int end)> ExpandIntervals(int start, int end)
    {
        if (start <= end)
            return [(start, end)];
        // wraps around year end
        return [(start, 366), (1, end)];
    }    
}
