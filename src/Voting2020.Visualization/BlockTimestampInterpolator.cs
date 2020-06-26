using System;

using Voting2020.Core;

namespace Voting2020.Visualization
{
	public sealed class BlockTimestampInterpolator
		: IBlockTimestampInterpolator
	{
		private readonly BlockGraphItem<TimeSpan>[] _blockGraphItems;

		public BlockTimestampInterpolator(BlockGraphItem<TimeSpan>[] blockGraphItems)
		{
			_blockGraphItems = blockGraphItems;
		}

		public TimeSpan GetBlockTimestamp(int blockNumber)
		{
			
			ref BlockGraphItem<TimeSpan> block = ref _blockGraphItems[0];
			if (blockNumber<= block.BlockNumber)
			{
				return block.Data;
			}
			block = ref _blockGraphItems[_blockGraphItems.Length - 1];
			if (blockNumber >= block.BlockNumber)
			{
				return block.Data;
			}
			int left = 0;
			int right = _blockGraphItems.Length - 1;

			while (right - left > 1)
			{
				var test = (left + right) / 2;
				block = ref _blockGraphItems[test];
				if (block.BlockNumber == blockNumber)
				{
					return block.Data;
				}
				if (block.BlockNumber < blockNumber)
				{
					left = test;
				}
				else
				{
					right = test;
				}
			}
			var timeDelta = _blockGraphItems[right].Data - _blockGraphItems[left].Data;
			var blockCount = _blockGraphItems[right].BlockNumber - _blockGraphItems[left].BlockNumber;
			timeDelta /= blockCount;
			var ret = _blockGraphItems[left].Data + timeDelta * (blockNumber - _blockGraphItems[left].BlockNumber);
			return ret;
		}
	}
}
