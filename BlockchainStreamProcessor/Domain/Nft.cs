namespace BlockchainStreamProcessor.Domain;

public class Nft
{
	public Nft(string tokenId)
	{
		TokenId = tokenId;
	}

	public int Id { get; set; } //PK
	public string TokenId { get; private set; }
	public string WalletAddress { get; private set; }

	public void AddToWallet(string walledAddr)
	{
		//TODO: this is a method because in a real world scenario more interesting things can happen here (ex: validations and other checks)
		WalletAddress = walledAddr;
	}

	public void Move(string toWallet)
	{
		//TODO: this again can be more complex in a rel world scenario especially if the domain model is more complicated 
		// there might be a Wallet entity etc.
		WalletAddress = toWallet;
	}
}