namespace AspireSample.Catalog.Api.Features.Products;

public record Product(Guid Id, string Name, string Sku, string Currency, decimal Quantity);
