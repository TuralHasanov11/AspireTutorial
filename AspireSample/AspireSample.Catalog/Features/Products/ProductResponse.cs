namespace AspireSample.Catalog.Api.Features.Products;

public record ProductResponse(Guid Id, string Name, string Sku, string Currency, decimal Quantity, IEnumerable<Link> Links);

public record PaginatedProductResponse(
    IEnumerable<Product> Products,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    IEnumerable<Link> Links
);
