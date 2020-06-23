using System;

namespace Voting2020.Core
{
	public sealed class EncryptedVoteRecord
	{
		public int VoteNumber { get; set; }
		public int DistrictNumber { get; set; }
		public string BlockchainAddress { get; set; }
		public int BlockNumber { get; set; }
		public TimeSpan Time { get; set; }

		public EncryptedVote EncryptedVote { get; set; }
	}
}
