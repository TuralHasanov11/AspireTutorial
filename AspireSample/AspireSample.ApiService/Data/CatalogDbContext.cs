using Microsoft.EntityFrameworkCore;

namespace AspireSample.ApiService.Data;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

}