using System;
using System.IO;
using System.Security.Permissions;

using ProtoBuf;

using Voting2020.Core;

namespace RawStoreBallotTxDecoder
{
	class Program
	{
		static void Main(string[] args)
		{
			var hexData = args[0];

			ReadOnlyMemory<byte> data = hexData.ParseHexEncodedValue();

			var signature = data.Slice(data.Length - 64);
			var messageWithHeader = data.Slice(0, data.Length - 64);
			var stream = new MemoryStream(messageWithHeader.ToArray());
			var header=ExonumMessageHeader.ReadFromStream(stream);
			var ballot = Serializer.Deserialize<TxStoreBallot>(stream);

			Console.WriteLine("Exonum message header ->");
			Console.WriteLine("\t Public Key: {0}", header.PublicKey.ToHexString());
			Console.WriteLine("\t Class: {0} ({1})", (byte) header.Class, header.Class);
			Console.WriteLine("\t Type: {0}", header.Type);
			Console.WriteLine("\t ServiceId: {0}", header.ServiceId);
			Console.WriteLine("\t MessageId: {0}", header.MessageId);
			Console.WriteLine("TxStoreBallot Payload ->");
			Console.WriteLine("\t VotingId: {0}", ballot.VotingId);
			Console.WriteLine("\t DistrictId: {0}", ballot.DistrictId);
			Console.WriteLine("\t TxEncryptedChoice ->");
			Console.WriteLine("\t\t EncryptedMessage: {0}", ballot.EncryptedChoice.EncryptedMessage.ToHexString());
			Console.WriteLine("\t\t nonce: {0}", ballot.EncryptedChoice.Nonce.Data.ToHexString());
			Console.WriteLine("\t\t public_key: {0}", ballot.EncryptedChoice.PublicKey.Data.ToHexString());

			Console.WriteLine("Signature: {0}", signature.ToArray().ToHexString());


			Console.ReadLine();
		}
	}
}
