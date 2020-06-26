using System;

namespace Voting2020.Visualization
{
	public readonly struct BlockGraphItem<T>
		: IEquatable<BlockGraphItem<T>>
		where T : struct, IEquatable<T>
	{
		private readonly int _blockNumber;
		private readonly T _data;

		public BlockGraphItem(int blockNumber, T data)
		{
			_blockNumber = blockNumber;
			_data = data;
		}

		public int BlockNumber
		{
			get { return _blockNumber; }
		}

		public T Data
		{
			get { return _data; }
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_blockNumber, _data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			if (ReferenceEquals(null, obj)) return false;
			if (obj is BlockGraphItem<T> other)
			{
				return (this._blockNumber == other._blockNumber) && this._data.Equals(other._data);
			}
			return false;
		}

		public bool Equals(BlockGraphItem<T> other)
		{
			return (this._blockNumber == other._blockNumber) && this._data.Equals(other._data);
		}
	}
}
