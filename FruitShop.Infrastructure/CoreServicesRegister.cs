using Microsoft.Extensions.DependencyInjection;

namespace FruitShop.Infrastructure;

public static class CoreServicesRegister
{
    public static void RegisterCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IStoreData, StoreData>();
    }
}
