using MeteoWeaver.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace MeteoWeaver.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CityWeatherSnapshot> CityWeatherSnapshots => Set<CityWeatherSnapshot>();
    public DbSet<HourlyForecastEntry> HourlyForecastEntries => Set<HourlyForecastEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CityWeatherSnapshot>()
            .HasMany(x => x.HourlyForecastEntries)
            .WithOne(x => x.CityWeatherSnapshot)
            .HasForeignKey(x => x.CityWeatherSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CityWeatherSnapshot>()
            .HasIndex(x => new { x.CityName, x.ImportedAtUtc });
    }
}
