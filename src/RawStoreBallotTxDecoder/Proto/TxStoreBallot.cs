
using ProtoBuf;

namespace RawStoreBallotTxDecoder
{
	[ProtoContract]
	class TxStoreBallot
	{
		[ProtoMember(1)]
		public string VotingId { get; set; }

		[ProtoMember(2)]
		public int DistrictId { get; set; }

		[ProtoMember(3)]
		public TxEncryptedChoice EncryptedChoice { get; set; }
	}
}
