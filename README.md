# Fruitshop Api

## Table of Contents
- [Approach] 
- [Projects] 
- [Testing]


## Approach

Domain-first design: domain entities encapsulate behaviour That is, the domain model has immutable properties which can only modified using behavioural methods. The domain model contains the buisness logic and validation with the premise the domain model should be valid at all times. Fruits are dynamic, new fruits maybe added using the Api. A fruit may have zero or more pricing strategies. If no strategies are present, the system will use a simple Qty*BasePrice to calcuate the item total. The command resposbility pattern was used to apply mulitple strategies to a product/fruit. Each fruit has unit of measure, validators for each unit of measure are acheived using the decorator pattern (UnitOfMeasureValidator attribute). New strategies can be added be inherting the base class PricingStrategy. 

## Projects

•	FruitShop.Api — the minimal API app/entry point (Program.cs) that:
>	Adds OpenAPI / Scalar API metadata and an ExceptionHandlingMiddleware.
>	Maps product and basket routes and seeds the store on startup.

•	Fruitshop.Domain — domain model (products/ store/ baskets/ pricing) including:
>	Fruit, FruitStore, FruitBasket, UnitOfMeasure.
>	Pricing domain types and strategies: Discount, BulkDiscountPricingStrategy, SeasonalPrice, SeasonalPricingStrategy (Strategy pattern used for pricing rules).
>	Domain guards / exceptions used for validation.

•	FruitShop.Infrastructure — runtime/in-memory store layer:
>	StoreData implements IStoreData: in-memory FruitStore, thread-safe list of FruitBaskets, seeding of default fruits and pricing strategies, APIs to add fruits, baskets, items and attach pricing strategies.
>	Uses a lock object to prevent concurrent updates to baskets.

•	Test projects:
>	FruitShop.Infrastructure.Tests, FruitShop.Domain.Tests, FruitShop.Api.Tests — xUnit tests verifying seeding, adding products, basket operations and error conditions.

## Testing

Unit tests have been created for domain and integration layers. Integration tests have been created to test Api. To change the default (seeded) fruits and strategies, the SeedData method in StoreData class can be changed. Calculation logic can be tested usng the api/baskets (post) end point which takes in a list of fruit & quantities. The fruits must be available in the store. 