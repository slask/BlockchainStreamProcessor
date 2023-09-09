using BlockchainStreamProcessor.Domain;
using BlockchainStreamProcessor.Infrastructure;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class MintCommandHandler : IRequestHandler<MintCommand, Result>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<MintCommandHandler> _logger;

	public MintCommandHandler(BspDatabaseContext db, ILogger<MintCommandHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result> Handle(MintCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var nft = new Nft(request.TokenId);
			nft.AddToWallet(request.Address);

			await _db.Nfts.AddAsync(nft, cancellationToken);
			await _db.SaveChangesAsync(cancellationToken);

			_logger.LogInformation("Minted token {TokenId}", nft.TokenId);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return Result.Fail($"Failed to mint: {ex.Message}");
		}
	}
}