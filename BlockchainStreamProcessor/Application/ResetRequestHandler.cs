using BlockchainStreamProcessor.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application;

public class ResetRequestHandler : IRequestHandler<ResetRequest, string>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<ResetRequestHandler> _logger;

	public ResetRequestHandler(BspDatabaseContext db, ILogger<ResetRequestHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<string> Handle(ResetRequest request, CancellationToken cancellationToken)
	{
		try
		{
			await _db.Database.ExecuteSqlRawAsync("DELETE FROM Nfts", cancellationToken: cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);

			_logger.LogInformation("Database cleared");
			return "Program was reset";
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return ex.Message;
		}
	}
}