using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using working_with_timescale_data.Models.ModelsEntity;

namespace working_with_timescale_data.Models.DbContextDir;

public class TimescaleDb: DbContext
{
    public DbSet<FileResultEntity> FileResults { get; set; } 
    public DbSet<FileResultEntity> FileResults { get; set; }
    public TimescaleDb(DbContextOptions<TimescaleDb> options):base (options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        String connect = "Host=localhost;Port=5432;Database=TimescaleDb;Username=postgres;Password=42924870";
        optionsBuilder.UseNpgsql();
    }
}
