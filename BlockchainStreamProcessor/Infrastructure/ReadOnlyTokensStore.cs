using BlockchainStreamProcessor.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlockchainStreamProcessor.Infrastructure;

class ReadOnlyTokensStore : IQueryStore
{
	private readonly BspDatabaseContext _db;

	public ReadOnlyTokensStore(BspDatabaseContext db)
	{
		_db = db;
	}

	public IQueryable<Nft> Nfts => _db.Nfts.AsNoTracking().AsQueryable();
}