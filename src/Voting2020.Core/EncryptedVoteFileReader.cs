using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace Voting2020.Core
{
	public sealed class EncryptedVoteFileReader
	{
		private readonly CsvParser<EncryptedVoteCsvRecord> _csvParser;

		public EncryptedVoteFileReader()
		{
			var csvParserOptions = new CsvParserOptions(true, ';');
			_csvParser = new CsvParser<EncryptedVoteCsvRecord>(csvParserOptions, new EncryptedVoteRecordMap());
		}

		private sealed class EncryptedVoteCsvRecord
		{
			/// <summary>
			/// Номер голосоа
			/// </summary>
			public int VoteNumber { get; set; }

			/// <summary>
			/// Избирательный округ
			/// </summary>
			public int District { get; set; }

			/// <summary>
			/// Адрес голоса в блокчейне
			/// </summary>
			public string BlockchainAddress { get; set; }

			/// <summary>
			/// Блок голоса
			/// </summary>
			public string BlockNumber { get; set; }

			/// <summary>
			/// Время записи блока
			/// </summary>
			public string WriteTime { get; set; }

			public string EncryptedVote { get; set; }
		}

		private sealed class EncryptedVoteRecordMap
			: CsvMapping<EncryptedVoteCsvRecord>
		{
			public EncryptedVoteRecordMap()
			{
				MapProperty(0, x => x.VoteNumber);
				MapProperty(1, x => x.District);
				MapProperty(2, x => x.BlockchainAddress);
				MapProperty(3, x => x.BlockNumber);
				MapProperty(4, x => x.WriteTime);
				MapProperty(5, x => x.EncryptedVote);
			}
		}


		public EncryptedVoteRecord[] ReadFromFile(string fileName)
		{
			var records = _csvParser.ReadFromFile(fileName, Encoding.UTF8).ToArray();

			var list = new List<EncryptedVoteCsvRecord>(records.Length);
			foreach (var record in records)
			{
				if (record.Error != null)
				{

				}
				list.Add(record.Result);
			}
			list.Sort((x, y) => x.VoteNumber - y.VoteNumber);

			EncryptedVoteRecord[] ret = new EncryptedVoteRecord[list.Count];

			TimeSpan? lastTime = null;

			TimeSpan currentShift = TimeSpan.Zero;

			TimeSpan dayShift = new TimeSpan(1, 0, 0, 0, 0);
			for (int i = 0; i < list.Count; i++)
			{
				var csvRecord = list[i];

				var time = TimeSpan.Parse(csvRecord.WriteTime);
				if (lastTime.HasValue)
				{
					if ((time - lastTime.Value) < TimeSpan.Zero)
					{
						currentShift += dayShift;
					}
				}
				var voteTime = time + currentShift;
				lastTime = time;

				var newRecord = new EncryptedVoteRecord()
				{
					VoteNumber = csvRecord.VoteNumber,
					DistrictNumber = csvRecord.District,
					BlockchainAddress = csvRecord.BlockchainAddress,
					BlockNumber = int.Parse(csvRecord.BlockNumber.AsSpan().TrimStart("#")),
					Time = voteTime,
					EncryptedVote = JsonSerializer.Deserialize<EncryptedVote>(csvRecord.EncryptedVote)
				};
				ret[i] = newRecord;
			}

			return ret;
		}
	}

}
