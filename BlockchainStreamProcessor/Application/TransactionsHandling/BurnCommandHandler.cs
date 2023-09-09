using BlockchainStreamProcessor.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class BurnCommandHandler : IRequestHandler<BurnCommand, Result>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<BurnCommandHandler> _logger;

	public BurnCommandHandler(BspDatabaseContext db, ILogger<BurnCommandHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result> Handle(BurnCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var nft = await _db.Nfts.FirstOrDefaultAsync(n => n.TokenId == request.TokenId, cancellationToken: cancellationToken);
			if (nft == null)
			{
				return Result.Fail("Token not found");
			}

			_db.Remove(nft);
			await _db.SaveChangesAsync(cancellationToken);

			_logger.LogInformation("Burned token {TokenId}", nft.TokenId);
			return Result.Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return Result.Fail($"Failed to burn: {ex.Message}");
		}
	}
}