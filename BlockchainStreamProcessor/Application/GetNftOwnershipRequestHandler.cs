using BlockchainStreamProcessor.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application;

public class GetNftOwnershipRequestHandler : IRequestHandler<GetNftOwnershipRequest, string>
{
	private readonly BspDatabaseContext _db;
	private readonly ILogger<GetNftOwnershipRequestHandler> _logger;

	public GetNftOwnershipRequestHandler(BspDatabaseContext db, ILogger<GetNftOwnershipRequestHandler> logger)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<string> Handle(GetNftOwnershipRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var nft = await _db.Nfts.FirstOrDefaultAsync(n => n.TokenId == request.TokenId, cancellationToken: cancellationToken);
			return nft == null
				? $"Token {request.TokenId} is not owned by any wallet"
				: $"Token {nft.TokenId} is owned by {nft.WalletAddress}";
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message);
			return ex.Message;
		}
	}
}