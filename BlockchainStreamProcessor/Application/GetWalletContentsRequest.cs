using MediatR;

namespace BlockchainStreamProcessor.Application;

public class GetWalletContentsRequest : IRequest<string>
{
	public GetWalletContentsRequest(string walletId)
	{
		WalletId = walletId;
	}

	public string WalletId { get; }
}