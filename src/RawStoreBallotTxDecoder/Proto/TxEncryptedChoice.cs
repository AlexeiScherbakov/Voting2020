
using ProtoBuf;

namespace RawStoreBallotTxDecoder
{
	[ProtoContract]
	class TxEncryptedChoice
	{
		[ProtoMember(1)]
		public byte[] EncryptedMessage { get; set; }
		[ProtoMember(2)]
		public SealedBoxNonce Nonce { get; set; }
		[ProtoMember(3)]
		public SealedBoxPublicKey PublicKey { get; set; }
	}
}
