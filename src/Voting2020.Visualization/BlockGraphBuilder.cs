using System;
using System.Linq;

using Voting2020.Core;

namespace Voting2020.Visualization
{
	public static class BlockGraphBuilder
	{
		public static BlockGraphItem<TimeSpan>[] BlockStart<T>(
			T[] data,
			Func<T, int> blockNumberSelector,
			Func<T, TimeSpan> blockTimestampSelector)
		{
			var blocks = data.Select(x => new BlockGraphItem<TimeSpan>(
				blockNumberSelector(x), blockTimestampSelector(x)))
				.Distinct()
				.OrderBy(x => x.BlockNumber)
				.ToArray();
			return blocks;
		}

		public static BlockGraphItem<TimeSpan>[] BlockTime<T>(
			T[] data,
			Func<T, int> blockNumberSelector,
			Func<T, TimeSpan> blockTimestampSelector)
		{
			var blocks = BlockStart(data, blockNumberSelector, blockTimestampSelector);
			return BlockTime(blocks);
		}

		public static BlockGraphItem<TimeSpan>[] BlockTime(
			BlockGraphItem<TimeSpan>[] blockstartGraphItems)
		{
			var startBlock = blockstartGraphItems[0].BlockNumber;
			var endBlock = blockstartGraphItems[blockstartGraphItems.Length - 1].BlockNumber;
			var totalBlocks = endBlock - startBlock;

			BlockGraphItem<TimeSpan>[] ret = new BlockGraphItem<TimeSpan>[totalBlocks];

			TimeSpan lastBlockStartTime = blockstartGraphItems[0].Data;
			int lastBlockNumber = blockstartGraphItems[0].BlockNumber;
			int blockCounter = 0;
			for (int i = 1; i < blockstartGraphItems.Length; i++)
			{
				ref BlockGraphItem<TimeSpan> currentBlock = ref blockstartGraphItems[i];

				var blocksTime = currentBlock.Data - lastBlockStartTime;
				var numberOfBlocks = currentBlock.BlockNumber - lastBlockNumber;

				var averageBlockTime = blocksTime / numberOfBlocks;

				for (int cnt = 0; cnt < numberOfBlocks; cnt++)
				{
					ret[blockCounter++] = new BlockGraphItem<TimeSpan>(lastBlockNumber + cnt, averageBlockTime);
				}
				lastBlockStartTime = currentBlock.Data;
				lastBlockNumber = currentBlock.BlockNumber;
			}

			return ret;
		}

		public static BlockGraphItem<int>[] TransactionPerBlock<T>(
			T[] data,
			Func<T, bool> filter,
			Func<T, int> blockNumberSelector)
		{
			var points = data
				.Where(x => filter(x))
				.GroupBy(x => blockNumberSelector(x), (key, votes) => new BlockGraphItem<int>(key, votes.Count()))
				.OrderBy(x => x.BlockNumber)
				.ToArray();
			return points;
		}
	}
}
