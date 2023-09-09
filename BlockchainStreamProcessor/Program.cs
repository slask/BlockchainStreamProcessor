using System.Reflection;
using BlockchainStreamProcessor;
using BlockchainStreamProcessor.Infrastructure;
using BlockchainStreamProcessor.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
	.ConfigureServices(ConfigureServices)
	.ConfigureServices(services =>
	{
		services.AddSingleton<EntryPoint>();
		services.AddEntityFrameworkSqlite().AddDbContext<BspDatabaseContext>();
	})
	.ConfigureAppConfiguration(builder => { builder.AddCommandLine(args); })
	.Build();

using var db = host.Services.GetService<BspDatabaseContext>();
db.Database.EnsureCreated();
db.Database.Migrate();

await host.Services
	.GetService<EntryPoint>()
	.Execute();

static void ConfigureServices(IServiceCollection services)
{
	services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
	services.AddTransient<ICommandParser, EnvironmentArgsCommandParser>();
	services.AddScoped<IQueryStore, ReadOnlyTokensStore>();
}