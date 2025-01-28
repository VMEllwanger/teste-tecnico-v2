using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Models.Reports;

namespace Thunders.TechTest.ApiService.Data
{
	[ExcludeFromCodeCoverage]
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<TollUsage> TollUsages { get; set; }
		public DbSet<HourlyCityReport> HourlyCityReports { get; set; }
		public DbSet<MonthlyPlazaReport> MonthlyPlazaReports { get; set; }
		public DbSet<VehicleTypeReport> VehicleTypeReports { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TollUsage>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
				entity.Property(e => e.Plaza).IsRequired().HasColumnType("varchar(450)");
				entity.Property(e => e.City).IsRequired().HasColumnType("varchar(450)");
				entity.Property(e => e.State).IsRequired().HasColumnType("varchar(2)");
				entity.Property(e => e.Amount).HasPrecision(18, 2);
				entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone")
					.HasDefaultValueSql("CURRENT_TIMESTAMP");
				entity.Property(e => e.DateTime).IsRequired().HasColumnType("timestamp with time zone");

				entity.HasIndex(e => e.CreatedAt);
				entity.HasIndex(e => e.City);
				entity.HasIndex(e => e.Plaza);
				entity.HasIndex(e => e.State);
			});

			modelBuilder.Entity<HourlyCityReport>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
				entity.Property(e => e.City).HasColumnType("varchar(450)");
				entity.Property(e => e.State).IsRequired().HasColumnType("varchar(2)");
				entity.Property(e => e.ReportId).IsRequired().HasColumnType("uuid");
				entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
				entity.Property(e => e.TotalVehicles);

				entity.Property(e => e.Date).HasColumnType("timestamp with time zone");
				entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone")
					.HasDefaultValueSql("CURRENT_TIMESTAMP");
				entity.HasIndex(e => new { e.City, e.Date, e.Hour });
				entity.HasIndex(e => e.State);
			});

			modelBuilder.Entity<MonthlyPlazaReport>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
				entity.Property(e => e.Plaza).HasColumnType("varchar(450)");
				entity.Property(e => e.City).HasColumnType("varchar(450)");
				entity.Property(e => e.State).HasColumnType("varchar(2)");
				entity.Property(e => e.ReportId).IsRequired().HasColumnType("uuid");
				entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
				entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone")
					.HasDefaultValueSql("CURRENT_TIMESTAMP");
				entity.HasIndex(e => new { e.Year, e.Month, e.Rank });
			});

			modelBuilder.Entity<VehicleTypeReport>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
				entity.Property(e => e.Plaza).HasColumnType("varchar(450)");
				entity.Property(e => e.City).HasColumnType("varchar(450)");
				entity.Property(e => e.State).HasColumnType("varchar(2)");
				entity.Property(e => e.ReportId).IsRequired().HasColumnType("uuid");
				entity.Property(e => e.Date).HasColumnType("timestamp with time zone");
				entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone")
					.HasDefaultValueSql("CURRENT_TIMESTAMP");
				entity.HasIndex(e => new { e.Plaza, e.Date, e.VehicleType });
			});
		}
	}
}
