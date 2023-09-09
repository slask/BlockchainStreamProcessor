using BlockchainStreamProcessor.Presentation;
using MediatR;

namespace BlockchainStreamProcessor.Application;

public class ReadTransactionsRequest : IRequest<string>
{
	public ReadTransactionsRequest(List<BlockchainTransaction> transactions)
	{
		BlockchainTransactions = transactions;
	}

	public List<BlockchainTransaction> BlockchainTransactions { get; }
}