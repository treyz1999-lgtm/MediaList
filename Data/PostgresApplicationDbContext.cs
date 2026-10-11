using Microsoft.EntityFrameworkCore;

namespace MediaWebApp.Data;

public class PostgresApplicationDbContext(DbContextOptions<PostgresApplicationDbContext> options)
    : ApplicationDbContext(options);
