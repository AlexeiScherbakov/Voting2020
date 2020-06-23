using System;

namespace Voting2019.Visualization
{
	public readonly struct BlockGraphItem<T>
		: IEquatable<BlockGraphItem<T>>
		where T : struct, IEquatable<T>
	{
		public readonly int BlockNumber;
		public readonly T Data;

		public BlockGraphItem(int blockNumber, T data)
		{
			BlockNumber = blockNumber;
			Data = data;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(BlockNumber, Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			if (ReferenceEquals(null, obj)) return false;
			if (obj is BlockGraphItem<T> other)
			{
				return (this.BlockNumber == other.BlockNumber) && this.Data.Equals(other.Data);
			}
			return false;
		}

		public bool Equals(BlockGraphItem<T> other)
		{
			return (this.BlockNumber == other.BlockNumber) && this.Data.Equals(other.Data);
		}
	}
}
