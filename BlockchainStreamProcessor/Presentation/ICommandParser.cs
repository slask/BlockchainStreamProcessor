using FluentResults;
using MediatR;

namespace BlockchainStreamProcessor.Presentation;

public interface ICommandParser
{
	Result<IRequest<string>> ParseProgramCommand();
}