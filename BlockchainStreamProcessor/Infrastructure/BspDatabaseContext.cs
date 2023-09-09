using BlockchainStreamProcessor.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BlockchainStreamProcessor.Infrastructure;

public class BspDatabaseContext : DbContext
{
	protected readonly IConfiguration Configuration;

	public BspDatabaseContext(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		// connect to sqlite database
		options.UseSqlite(Configuration.GetConnectionString("TokensDbConnString"));
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Nft>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.TokenId, "IX_TokenId").IsUnique();
		});
	}

	public DbSet<Nft> Nfts { get; set; }
}