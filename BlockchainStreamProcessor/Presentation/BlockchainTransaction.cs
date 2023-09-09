namespace BlockchainStreamProcessor.Presentation;

public class BlockchainTransaction
{
	public string Type { get; set; }
	public string TokenId { get; set; }
	public string Address { get; set; }
	public string From { get; set; }
	public string To { get; set; }
}