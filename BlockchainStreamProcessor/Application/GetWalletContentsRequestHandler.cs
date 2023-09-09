using BlockchainStreamProcessor.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application;

public class GetWalletContentsRequestHandler : IRequestHandler<GetWalletContentsRequest, string>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<GetWalletContentsRequestHandler> _logger;

	public GetWalletContentsRequestHandler(BspDatabaseContext db, ILogger<GetWalletContentsRequestHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<string> Handle(GetWalletContentsRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var nfts = await _db.Nfts.Where(n => n.WalletAddress == request.WalletId).ToListAsync(cancellationToken);
			return nfts.Count == 0
				? $"Wallet {request.WalletId} holds no tokens"
				: $"Wallet {request.WalletId} contains {nfts.Count} tokens:{Environment.NewLine}{string.Join("\n", nfts.Select(n => n.TokenId))}";
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return ex.Message;
		}
	}
}