using System;
using System.Linq;
using Xunit;
using FruitShop.Domain.Pricing;

namespace FruitShop.Domain.Tests.Pricing
{
    public class SeasonalPricingStrategyTests
    {
        [Fact]
        public void AddSeasonalPrice_AddsPrice_WhenNoOverlap()
        {
            var summer = SeasonalPrice.New("Summer", 6, 1, 8, 31, 1.2m);
            var winter = SeasonalPrice.New("Winter", 12, 1, 2, 28, 0.9m);

            var strategy = SeasonalPricingStrategy.New(summer);
            strategy.AddSeasonalPrice(winter);

            Assert.Equal(2, strategy.Prices.Count);
            Assert.Contains(strategy.Prices, p => p.SeasonName == "Summer");
            Assert.Contains(strategy.Prices, p => p.SeasonName == "Winter");
        }

        [Fact]
        public void AddSeasonalPrice_Throws_WhenDuplicateSeasonName()
        {
            var spring = SeasonalPrice.New("Spring", 3, 1, 5, 31, 1.1m);
            var strategy = SeasonalPricingStrategy.New(spring);

            var duplicate = SeasonalPrice.New("Spring", 6, 1, 8, 31, 1.0m);

            Assert.Throws<BadRequestException>(() => strategy.AddSeasonalPrice(duplicate));
        }

        [Fact]
        public void AddSeasonalPrice_Throws_WhenOverlapWithExisting()
        {
            var first = SeasonalPrice.New("First", 1, 1, 6, 30, 1.0m);
            var strategy = SeasonalPricingStrategy.New(first);

            // Overlaps: June 1 is within Jan 1 - Jun 30
            var overlapping = SeasonalPrice.New("Overlap", 6, 1, 7, 1, 1.0m);

            Assert.Throws<BadRequestException>(() => strategy.AddSeasonalPrice(overlapping));
        }

        [Fact]
        public void RemoveSeasonalPrice_Throws_WhenNotFound()
        {
            var price = SeasonalPrice.New("Only", 1, 1, 12, 31, 1.0m);
            var strategy = SeasonalPricingStrategy.New(price);

            Assert.Throws<NotFoundException>(() => strategy.RemoveSeasonalPrice("Missing"));
        }

        [Fact]
        public void RemoveSeasonalPrice_Throws_WhenLastRemaining()
        {
            var price = SeasonalPrice.New("Sole", 1, 1, 12, 31, 1.0m);
            var strategy = SeasonalPricingStrategy.New(price);

            Assert.Throws<BadRequestException>(() => strategy.RemoveSeasonalPrice("Sole"));
        }

        [Fact]
        public void AdjustSeasonalPrice_Replaces_WhenNoOverlap()
        {
            var a = SeasonalPrice.New("A", 1, 1, 3, 31, 1.0m);
            var b = SeasonalPrice.New("B", 4, 1, 6, 30, 1.0m);
            var strategy = SeasonalPricingStrategy.New(a);
            strategy.AddSeasonalPrice(b);

            var updatedA = SeasonalPrice.New("A", 1, 1, 3, 31, 1.5m);
            strategy.AdjustSeasonalPrice("A", updatedA);

            var found = strategy.Prices.FirstOrDefault(p => p.SeasonName == "A");
            Assert.NotNull(found);
            Assert.Equal(1.5m, found.PriceMultiplier);
        }

        [Fact]
        public void AdjustSeasonalPrice_Throws_WhenUpdatedOverlapsOther()
        {
            var a = SeasonalPrice.New("A", 1, 1, 3, 31, 1.0m);
            var b = SeasonalPrice.New("B", 4, 1, 6, 30, 1.0m);
            var strategy = SeasonalPricingStrategy.New(a);
            strategy.AddSeasonalPrice(b);

            // Make A overlap B by extending A into April
            var updatedA = SeasonalPrice.New("A", 1, 1, 4, 15, 1.0m);

            Assert.Throws<BadRequestException>(() => strategy.AdjustSeasonalPrice("A", updatedA));
        }
    }
}