using System;

namespace Voting2020.Core
{
	public interface IBlockTimestampInterpolator
	{
		TimeSpan GetBlockTimestamp(int blockNumber);
	}
}
