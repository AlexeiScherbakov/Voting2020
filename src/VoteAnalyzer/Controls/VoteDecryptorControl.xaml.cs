using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Extensions.Primitives;

using Voting2020.Core;

namespace VoteAnalyzer.Controls
{
	/// <summary>
	/// Interaction logic for VoteDecryptorControl.xaml
	/// </summary>
	public partial class VoteDecryptorControl
		: UserControl
	{
		private EncryptedVoteRecord[] _voteRecords;

		public VoteDecryptorControl()
		{
			InitializeComponent();
		}


		public void LoadRecords(EncryptedVoteRecord[] voteRecords)
		{
			_voteRecords = voteRecords;
		}


		private Dictionary<uint, int> Decrypt()
		{
			uint[] decryptedVotes = new uint[_voteRecords.Length];

			VoteDecoder decoder = new VoteDecoder("db77d62fc88726f1c5a6b79b003b3bca83349e334337de84bd17344ab601db74".ParseHexEncodedValue());

			Dictionary<uint, int> counters = new Dictionary<uint, int>();
			_current = 0;
			Parallel.For(0, _voteRecords.Length, i =>
			{
				bool ok=decoder.DecodeEndVerify(_voteRecords[i].EncryptedVote, out var decoded);
				var value = BitConverter.ToUInt32(decoded);
				lock (counters)
				{
					if (!counters.TryGetValue(value, out var count))
					{
						counters.Add(value, 1);
					}
					else
					{
						counters[value] = count + 1;
					}
				}
				Interlocked.Increment(ref _current);
			});

			return counters;
		}

		private int _current;

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			_busyIndicator.IsBusy = true;

			var timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, delegate
			  {
				  _busyIndicator.BusyContent = _current + "/" + _voteRecords.Length;
			  }, Dispatcher);
			timer.Start();

			var counters=await Task.Factory.StartNew(delegate
			{
				return Decrypt();
			});
			timer.Stop();
			_busyIndicator.IsBusy = false;

			StringBuilder result = new StringBuilder();
			foreach(var pair in counters)
			{
				result.AppendLine(pair.Key + ": " + pair.Value);
			}
			txt.Text = result.ToString();
		}
	}
}
