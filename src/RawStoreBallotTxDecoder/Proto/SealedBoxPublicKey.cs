
using ProtoBuf;

namespace RawStoreBallotTxDecoder
{
	[ProtoContract]
	class SealedBoxPublicKey
	{
		[ProtoMember(1)]
		public byte[] Data { get; set; }
}
}
