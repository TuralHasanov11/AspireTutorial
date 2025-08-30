namespace AspireSample.Catalog.Api.Features.Products;

public static class Endpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products", (ILinkService linkService) =>
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid(), "Laptop", "SKU123", "USD", 10),
                new Product(Guid.NewGuid(), "Phone", "SKU456", "USD", 25),
                new Product(Guid.NewGuid(), "Tablet", "SKU789", "USD", 15)
            };

            var hateoasProducts = new PaginatedProductResponse(
                    products,
                    products.Count,
                    1, // Page number
                    25, // Page size
                    1, // Total pages
                    [
                        linkService.Generate("GetProducts", null, "self", Link.GET),
                        linkService.Generate("GetProducts", new { page = 2 }, "next-page", Link.GET),
                        linkService.Generate("GetProducts", new { page = 1 }, "first-page", Link.GET),
                        linkService.Generate("GetProducts", new { page = 3 }, "last-page", Link.GET),
                        linkService.Generate("GetProducts", new { page = 1 }, "previous-page", Link.GET)
                    ]
                );

            return Results.Ok(hateoasProducts);
        }).WithName("GetProducts");

        app.MapGet("/api/products/{id}", (ILinkService linkService, Guid id) =>
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid(), "Laptop", "SKU123", "USD", 10),
                new Product(Guid.NewGuid(), "Phone", "SKU456", "USD", 25),
                new Product(Guid.NewGuid(), "Tablet", "SKU789", "USD", 15)
            };
            var product = products.FirstOrDefault(p => p.Id == id);
            return product is null ? Results.NotFound() : Results.Ok(ToHateoas(product, linkService));
        }).WithName("GetProductById");

        app.MapPost("/api/products", (ILinkService linkService, Product product) =>
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid(), "Laptop", "SKU123", "USD", 10),
                new Product(Guid.NewGuid(), "Phone", "SKU456", "USD", 25),
                new Product(Guid.NewGuid(), "Tablet", "SKU789", "USD", 15)
            };
            var newProduct = product with { Id = Guid.NewGuid() };
            products.Add(newProduct);
            return Results.Created($"/api/products/{newProduct.Id}", ToHateoas(newProduct, linkService));
        }).WithName("CreateProduct");

        app.MapPut("/api/products/{id}", (ILinkService linkService, Guid id, Product updated) =>
        {
            var products = new List<Product>
            {
                new Product(Guid.NewGuid(), "Laptop", "SKU123", "USD", 10),
                new Product(Guid.NewGuid(), "Phone", "SKU456", "USD", 25),
                new Product(Guid.NewGuid(), "Tablet", "SKU789", "USD", 15)
            };
            var index = products.FindIndex(p => p.Id == id);
            if (index == -1)
            {
                return Results.NotFound();
            }
            products[index] = updated with { Id = id };
            return Results.Ok(ToHateoas(products[index], linkService));
        }).WithName("UpdateProduct");

        app.MapDelete("/api/products/{id}", (ILinkService linkService, Guid id) =>
        {
            var products = new List<Product>
            {
                new(Guid.NewGuid(), "Laptop", "SKU123", "USD", 10),
                new(Guid.NewGuid(), "Phone", "SKU456", "USD", 25),
                new(Guid.NewGuid(), "Tablet", "SKU789", "USD", 15)
            };
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return Results.NotFound();
            }
            products.Remove(product);
            return Results.NoContent();
        }).WithName("DeleteProduct");
    }

    private static ProductResponse ToHateoas(Product product, ILinkService linkService)
    {
        var links = new List<Link>
        {
            linkService.Generate("GetProductById", new { id = product.Id }, "self", Link.GET),
            linkService.Generate("UpdateProduct", new { id = product.Id }, "update", Link.PUT),
            linkService.Generate("DeleteProduct", new { id = product.Id }, "delete", Link.DELETE),
            linkService.Generate("GetProducts", null, "list", Link.GET),
            linkService.Generate("CreateProduct", null, "create", Link.POST)
        };
        return new ProductResponse(product.Id,
            product.Name,
            product.Sku,
            product.Currency,
            product.Quantity,
            links);
    }
}
