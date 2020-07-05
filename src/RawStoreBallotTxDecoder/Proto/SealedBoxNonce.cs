
using ProtoBuf;

namespace RawStoreBallotTxDecoder
{
	[ProtoContract]
	class SealedBoxNonce
	{
		[ProtoMember(1)]
		public byte[] Data { get; set; }
	}
}
