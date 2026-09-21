using FruitShop.Domain.Products;

namespace FruitShop.Domain.Baskets.Validation;

internal static class UnitOfMeasureValidatorFactory
{
    internal static List<IUnitOfMeasureValidator> GetValidators(Fruit fruit)
    {
        var validators = new List<IUnitOfMeasureValidator>();

        var implementations = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(assembly => assembly.GetTypes())
                                .Where(type => typeof(IUnitOfMeasureValidator).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                                .ToList();

        foreach (var type in implementations)
        {
            var attribute = (UnitOfMeasureValidatorAttribute?)Attribute.GetCustomAttribute(type, typeof(UnitOfMeasureValidatorAttribute));
            
            if (attribute != null && attribute.UnitOfMeasure == fruit.UnitOfMeasure)
            {
                var validator = (IUnitOfMeasureValidator)Activator.CreateInstance(type)!;
                validators.Add(validator);
            }

        }
        return validators;
    }
}
