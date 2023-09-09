using FluentResults;
using MediatR;

namespace BlockchainStreamProcessor.Application.TransactionsHandling;

public class TransferCommand : IRequest<Result>
{
	public TransferCommand(string tokenId, string from, string to)
	{
		TokenId = tokenId;
		From = from;
		To = to;
	}

	public string TokenId { get; }
	public string From { get; }
	public string To { get; }
}