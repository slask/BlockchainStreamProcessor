using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BlockchainStreamProcessor.Application;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlockchainStreamProcessor.Presentation;

// this class relies more on infra parts that are not mock-able (Environment, File etc.) so i decided to exclude from coverage*
//
// *Technically it would be possible to push Environment and File behind an ICommandArgsProvider and IFileProvider respectively
// and mock only those and create unit tests for the rest of this class as well. 
[ExcludeFromCodeCoverage]
public class EnvironmentArgsCommandParser : ICommandParser
{
	private readonly ILogger<EnvironmentArgsCommandParser> _logger;


	public EnvironmentArgsCommandParser(ILogger<EnvironmentArgsCommandParser> logger)
	{
		_logger = logger;
	}

	public Result<IRequest<string>> ParseProgramCommand()
	{
		var args = Environment.GetCommandLineArgs();

		if (args.Length <= 1)
		{
			return Result.Fail("No command was given");
		}

		string parameter = args[1];
		string value = args.Length >= 3 ? args[2] : null;

		// this is the extension point where other types of CLI parameters can be added
		switch (parameter.ToLowerInvariant())
		{
			case Parameters.ReadFile:
			{
				return ReadTransactionsFromFile(value);
			}

			case Parameters.ReadInline:
			{
				return ReadTransactionsFromCmdLine(args);
			}

			case Parameters.Nft:
			{
				return CreateNftRequest(value);
			}

			case Parameters.Wallet:
			{
				return CreateWalletRequest(value);
			}

			case Parameters.Reset:
			{
				return Result.Ok((IRequest<string>)new ResetRequest());
			}

			default:
				return Result.Fail($"Unknown parameter: {parameter}");
		}
	}

	private Result<IRequest<string>> CreateWalletRequest(string value)
	{
		//TODO: maybe here add some input validation and return a Failed Result if the ID is not in the correct format etc.
		return Result.Ok((IRequest<string>)new GetWalletContentsRequest(value));
	}

	private static Result<IRequest<string>> CreateNftRequest(string value)
	{
		//TODO: maybe here add some input validation and return a Failed Result if the ID is not in the correct format etc.
		return Result.Ok((IRequest<string>)new GetNftOwnershipRequest(value));
	}

	private Result<IRequest<string>> ReadTransactionsFromCmdLine(string[] args)
	{
		// Oh gosh , this is more difficult than I expected, I drop it for now, Don't use it :D
		try
		{
			var readInlineParamValue = string.Join("", args.Skip(2));
			if (string.IsNullOrWhiteSpace(readInlineParamValue))
			{
				return Result.Fail("The value must not be empty");
			}

			List<BlockchainTransaction> transactions = new List<BlockchainTransaction>();
			if (IsJsonArray(readInlineParamValue))
			{
				transactions = JsonSerializer.Deserialize<List<BlockchainTransaction>>(readInlineParamValue);
			}
			else if (IsJsonObject(readInlineParamValue))
			{
				transactions.Add(JsonSerializer.Deserialize<BlockchainTransaction>(readInlineParamValue));
			}
			else
			{
				return Result.Fail("Malformed messages");
			}

			return Result.Ok((IRequest<string>)new ReadTransactionsRequest(transactions));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error reading file {Message}", ex.Message);
			return Result.Fail(ex.Message);
		}
	}

	private bool IsJsonObject(string readInlineParamValue)
	{
		return readInlineParamValue.Trim().StartsWith("{"); 
		// differentiating between obj and array is a bit odd but don't have a bette idea right now
		// i would push back on the requirement and always ask for an array even if it has only one obj inside
		// that way reading from a file or inline is consistent
	}

	private static bool IsJsonArray(string readInlineParamValue)
	{
		return readInlineParamValue.Trim().StartsWith("[");
	}

	private Result<IRequest<string>> ReadTransactionsFromFile(string filePath)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return Result.Fail("Must provide a file name.");
			}

			if (!File.Exists(filePath))
			{
				return Result.Fail("File not found.");
			}

			string fileContents = File.ReadAllText(filePath);
			List<BlockchainTransaction> transactions = JsonSerializer.Deserialize<List<BlockchainTransaction>>(fileContents);

			return Result.Ok((IRequest<string>)new ReadTransactionsRequest(transactions));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error reading file {Message}", ex.Message);
			return Result.Fail(ex.Message);
		}
	}
}