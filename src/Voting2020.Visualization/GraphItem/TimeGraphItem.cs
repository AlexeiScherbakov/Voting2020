using System;

namespace Voting2019.Visualization
{
	public readonly struct TimeGraphItem<T>
		: IEquatable<TimeGraphItem<T>>
		where T : struct, IEquatable<T>
	{
		public readonly TimeSpan Timestamp;
		public readonly T Data;

		public TimeGraphItem(TimeSpan timestamp, T data)
		{
			Timestamp = timestamp;
			Data = data;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Timestamp, Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			if (ReferenceEquals(null, obj)) return false;
			if (obj is TimeGraphItem<T> other)
			{
				return (this.Timestamp == other.Timestamp) && this.Data.Equals(other.Data);
			}
			return false;
		}

		public bool Equals(TimeGraphItem<T> other)
		{
			return (this.Timestamp == other.Timestamp) && this.Data.Equals(other.Data);
		}
	}
}
