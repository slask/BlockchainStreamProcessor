using BlockchainStreamProcessor.Presentation;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor;

public class EntryPoint
{
	private readonly ILogger<EntryPoint> _logger;
	private readonly IMediator _mediator;
	private readonly ICommandParser _parser;

	public EntryPoint(ILogger<EntryPoint> logger, IMediator mediator, ICommandParser parser)
	{
		_parser = parser;
		_mediator = mediator;
		_logger = logger;
	}

	public async Task Execute()
	{
		_logger.LogInformation("Executing...");

		Result<IRequest<string>> result = _parser.ParseProgramCommand();
		if (result.IsFailed)
		{
			Console.WriteLine($"Failed to parse command: {result.Errors.First().Message}");
			return;
		}

		var response = await _mediator.Send(result.Value);
		Console.WriteLine($"{response}");
	}
}