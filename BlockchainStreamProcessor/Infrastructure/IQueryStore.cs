using BlockchainStreamProcessor.Domain;

namespace BlockchainStreamProcessor.Infrastructure;

public interface IQueryStore
{
	IQueryable<Nft> Nfts { get; }
}