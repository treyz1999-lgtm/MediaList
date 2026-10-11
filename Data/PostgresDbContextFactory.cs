using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MediaWebApp.Data;

public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresApplicationDbContext>
{
    public PostgresApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=medialist_design;Username=postgres;Password=unused")
            .Options;
        return new PostgresApplicationDbContext(options);
    }
}
