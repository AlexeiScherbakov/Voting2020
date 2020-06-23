using NUnit.Framework;
using System.Linq;

namespace Voting2020.Core.Tests
{
	public class EncryptedVoteFileReaderTests
	{

		[Test]
		public void ReadAllTest()
		{
			var voteReader = new EncryptedVoteFileReader();
			var data = voteReader.ReadFromFile("TestData\\ballots_decrypted_2020-06-19T17_37_00.csv");

			// Проверяем факт наличия первой записи
			// "85388";52;"0xd20f7dd3bfb77b7336d262995d24939c4db6296f1226d80950c5400aaed35404";"#168941";"15:38:27";"{""message"":""341710d93b521ab992d1261262957bbbae7911ac"",""nonce"":""bff354082494b923965c10429a7d9971efee5be3090f54ca"",""public_key"":""d323c5b6bf3ffd0867f737b16a3c44e547837ebe73ca7767e15b38963b27a05d""}";"2212294583";"#172233";"17:28:37";"d20f7dd3bfb77b7336d262995d24939c4db6296f1226d80950c5400aaed354040000e90306000a403134666534633962623037336431613164393264363065626139313033353165313339643562373637643237653136646232396535303239323838366234383310341a560a14341710d93b521ab992d1261262957bbbae7911ac121a0a18bff354082494b923965c10429a7d9971efee5be3090f54ca1a220a20d323c5b6bf3ffd0867f737b16a3c44e547837ebe73ca7767e15b38963b27a05dd40cbd489eaae0b86926415d4bc4bc418df323c1e24a1c2d1fffd2a7611f10fda4347613060487f110fb265c7c13fb78600368bf66e79c313bb6476e83632801"
			var firstRecord = data.Where(x => x.VoteNumber == 85388).Single();
			Assert.AreEqual(52, firstRecord.DistrictNumber);
			Assert.AreEqual(168941, firstRecord.BlockNumber);
			// Проверяем факт наличия последней записи
			// "85197";77;"0x184f3780a667dd316df77166dda4ec3b38bf531b0ec851c3ed0c2d10589df55f";"#168693";"15:31:52";"{""message"":""79842eb491afb0ea7cc21f3ebdbce8ed75800def"",""nonce"":""795435e800c610e1645672c042e23418ed4d5af485f5e970"",""public_key"":""a8ec1ce1ed4644ec3e8e15d24e11075aa807a5c2addb1a041c1e5e610d0ea85e""}"
			var lastRecord = data.Where(x => x.VoteNumber == 85197).Single();
			Assert.AreEqual(77, lastRecord.DistrictNumber);
			Assert.AreEqual(168693, lastRecord.BlockNumber);
		}
	}
}