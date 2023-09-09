using FluentResults;
using MediatR;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class BurnCommand : IRequest<Result>
{
	public BurnCommand(string tokenId)
	{
		TokenId = tokenId;
	}

	public string TokenId { get; }
}