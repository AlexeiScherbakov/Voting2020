using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Voting2020.Core
{
	public sealed class UnencryptedVoteFileReader
	{
		private readonly CsvParser<UnencryptedVoteRecord> _csvParser;

		public UnencryptedVoteFileReader()
		{
			var csvParserOptions = new CsvParserOptions(true, ',');
			_csvParser = new CsvParser<UnencryptedVoteRecord>(csvParserOptions, new UnencryptedVoteRecordMap());
		}

		private sealed class UnencryptedVoteRecord
		{
			/// <summary>
			/// Номер голосоа
			/// </summary>
			public int VoteNumber;

			/// <summary>
			/// Избирательный округ
			/// </summary>
			public int District;

			/// <summary>
			/// Адрес голоса в блокчейне
			/// </summary>
			public string BlockchainAddress;

			/// <summary>
			/// Блок голоса
			/// </summary>
			public string BlockNumber;

			/// <summary>
			/// Время записи блока
			/// </summary>
			public string WriteTime;

			public string EncryptedVote;

			public string DecryptedVote;

			public string DecryptionBlock;

			public string DecryptionBlockTime;

			public string Transaction;
		}

		private sealed class UnencryptedVoteRecordMap
			: CsvMapping<UnencryptedVoteRecord>
		{
			public UnencryptedVoteRecordMap()
			{
				MapProperty(0, x => x.VoteNumber);
				MapProperty(1, x => x.District);
				MapProperty(2, x => x.BlockchainAddress);
				MapProperty(3, x => x.BlockNumber);
				MapProperty(4, x => x.WriteTime);
				MapProperty(5, x => x.EncryptedVote);
				MapProperty(6, x => x.DecryptedVote);
				MapProperty(7, x => x.DecryptionBlock);
				MapProperty(8, x => x.DecryptionBlockTime);
				MapProperty(9, x => x.Transaction);
			}
		}
	}
}
