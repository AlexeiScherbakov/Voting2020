namespace Voting2020.Core
{
	public interface IBlockChainBlockRange
	{
		int MinBlockNumber { get; }
		int MaxBlockNumber { get; }
	}
}
