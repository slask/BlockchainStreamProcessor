using MediatR;

namespace BlockchainStreamProcessor.Application;

public class GetNftOwnershipRequest : IRequest<string>
{
	public GetNftOwnershipRequest(string tokenId)
	{
		TokenId = tokenId;
	}

	public string TokenId { get; }
}