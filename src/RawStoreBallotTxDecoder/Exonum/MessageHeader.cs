using System.IO;
using System.ServiceModel.Channels;

namespace RawStoreBallotTxDecoder
{
	public sealed class ExonumMessageHeader
	{


		public ExonumMessageHeader()
		{

		}

		public byte[] PublicKey { get; set; }
		public MessageClass Class { get; set; }
		public byte Type { get; set; }
		public ushort ServiceId { get; set; }
		public ushort MessageId { get; set; }

		public static ExonumMessageHeader ReadFromStream(Stream s)
		{
			BinaryReader r = new BinaryReader(s);
			var header = new ExonumMessageHeader();
			header.PublicKey = r.ReadBytes(32);
			header.Class = (MessageClass) r.ReadByte();
			header.Type = r.ReadByte();
			header.ServiceId = r.ReadUInt16();
			header.MessageId = r.ReadUInt16();
			return header;
		}
	}
}
