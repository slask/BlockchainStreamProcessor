using BlockchainStreamProcessor.Application.TransactionsHandling;
using BlockchainStreamProcessor.Presentation;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Application;

public class ReadTransactionsRequestHandler : IRequestHandler<ReadTransactionsRequest, string>
{
	private readonly IMediator _mediator;
	private readonly ILogger<ReadTransactionsRequestHandler> _logger;

	public ReadTransactionsRequestHandler(IMediator mediator, ILogger<ReadTransactionsRequestHandler> logger)
	{
		_logger = logger;
		_mediator = mediator;
	}

	public async Task<string> Handle(ReadTransactionsRequest request, CancellationToken cancellationToken)
	{
		int successfullyApplied = 0;
		foreach (var transaction in request.BlockchainTransactions)
		{
			object transactionCommand = GetTransactionCommand(transaction);
			if (transactionCommand == null)
			{
				//ignore unknown commands
				continue;
			}

			var result = await _mediator.Send(transactionCommand, cancellationToken) as Result;
			if (result.IsFailed)
			{
				// what needs to be done here can depend on a larger context:
				// - interrupt the whole flow and return an error,
				// - ignore and continue with next transaction etc.
				// to simplify i decided to just ignore and continue
				_logger.LogError("Failed to apply command {Type} because {Message}", transaction.Type, result.Errors.First().Message);
				continue;
			}

			successfullyApplied++;
		}

		return $"Applied {successfullyApplied} of {request.BlockchainTransactions.Count} total transaction(s).";
	}

	private object GetTransactionCommand(BlockchainTransaction transaction)
	{
		return transaction.Type.ToLowerInvariant() switch
		{
			TransactionTypes.Mint => new MintCommand(transaction.TokenId, transaction.Address),
			TransactionTypes.Burn => new BurnCommand(transaction.TokenId),
			TransactionTypes.Transfer => new TransferCommand(transaction.TokenId, transaction.From, transaction.To),
			_ => null
		};
	}
}