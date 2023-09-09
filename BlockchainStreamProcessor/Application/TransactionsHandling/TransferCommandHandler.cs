using BlockchainStreamProcessor.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<TransferCommandHandler> _logger;

	public TransferCommandHandler(BspDatabaseContext db, ILogger<TransferCommandHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var nft = await _db.Nfts.FirstOrDefaultAsync(n => n.TokenId == request.TokenId, cancellationToken: cancellationToken);
			if (nft == null)
			{
				return Result.Fail("Token not found");
			}

			nft.Move(request.To);
			await _db.SaveChangesAsync(cancellationToken);

			_logger.LogInformation("Transferred token {TokenId}", nft.TokenId);
			return Result.Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return Result.Fail($"Failed to burn: {ex.Message}");
		}
	}
}