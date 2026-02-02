namespace working_with_timescale_data.Models.DbContextDir;

public class TimescaleDb: DbContext
{
    public DbSet<MeasurementEntity> Measurements { get; set; } 
    public DbSet<FileResultEntity> FileResults { get; set; }

    static TimescaleDb()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public TimescaleDb(DbContextOptions<TimescaleDb> options):base (options)
    {
        //Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        String connect = "Host=localhost;Port=5432;Database=TimescaleDb;Username=postgres;Password=42924870";
        optionsBuilder.UseNpgsql(connect);
    }
}
