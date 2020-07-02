
using NaCl;

namespace Voting2020.Core
{
	public sealed class VoteDecoder
	{
		private byte[] _privateKey;

		public VoteDecoder(byte[] privateKey)
		{
			_privateKey = privateKey;
		}


		public bool DecodeEndVerify(EncryptedVote vote,out byte[] decoded)
		{
			Curve25519XSalsa20Poly1305 box = new Curve25519XSalsa20Poly1305(_privateKey, vote.PublicKey.ParseHexEncodedValue());
			var encryptedMessage = vote.Message.ParseHexEncodedValue();
			var decryptedMessage= new byte[encryptedMessage.Length - XSalsa20Poly1305.TagLength];
			var ret=box.TryDecrypt(decryptedMessage, encryptedMessage, vote.Nonce.ParseHexEncodedValue());
			decoded = decryptedMessage;
			return ret;
		}
	}
}
