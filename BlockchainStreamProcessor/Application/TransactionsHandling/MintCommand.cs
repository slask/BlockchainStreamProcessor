using FluentResults;
using MediatR;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class MintCommand : IRequest<Result>
{
	public MintCommand(string tokenId, string address)
	{
		TokenId = tokenId;
		Address = address;
	}

	public string TokenId { get; }
	public string Address { get; }
}