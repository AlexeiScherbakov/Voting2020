using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Voting2020.Core
{
	public interface IVoteObject
	{
		int VoteNumber { get; set; }
	}

	public interface IVoteInBlockObject
		: IVoteObject
	{
		int BlockNumber { get; set; }
		TimeSpan Time { get; set; }
	}


	public sealed class VoteObjectComparer<T>
		: Comparer<T>
		where T : IVoteObject
	{
		public override int Compare([AllowNull] T x, [AllowNull] T y)
		{
			if ((x is null)||(y is null))
			{
				// это не позволяется
				throw new InvalidOperationException();
			}

			return y.VoteNumber - x.VoteNumber;
		}

		public static readonly VoteObjectComparer<T> Instance = new VoteObjectComparer<T>();
	}
}
