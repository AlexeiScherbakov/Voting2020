using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Voting2020.Visualization;
using Voting2020.Core;

namespace VoteAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
		: Window
	{

		private BlockTimePlotModelDrawer _blockTimestamp;
		private BlockTimePlotModelDrawer _averageBlockTime;
		private TransactionPerBlockModelDrawer _transactionPerBlock;
		private TransactionPerBlockTimelineModelDrawer _transactionPerBlockTimeline;
		private TransactionPerBlockTimelineModelDrawer _transactionPerDistrictPerBlockTimeline;

		public MainWindow()
		{
			InitializeComponent();

			_blockTimestamp = new BlockTimePlotModelDrawer(true);
			_averageBlockTime = new BlockTimePlotModelDrawer(false);
			_transactionPerBlock = new TransactionPerBlockModelDrawer();
			_transactionPerBlockTimeline = new TransactionPerBlockTimelineModelDrawer();
			_transactionPerDistrictPerBlockTimeline = new TransactionPerBlockTimelineModelDrawer();

			_blockTimestampPlotView.LoadModelDrawer(_blockTimestamp);
			_blockTimePlotView.LoadModelDrawer(_averageBlockTime);
			_transactionPerBlockPlotView.LoadModelDrawer(_transactionPerBlock);
			_transactionPerBlockTimelinePlotView.LoadModelDrawer(_transactionPerBlockTimeline);
			_transactionPerDistrictAndBlockTimelinePlotView.LoadModelDrawer(_transactionPerDistrictPerBlockTimeline);

			this.Loaded += MainWindow_Loaded;
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			_busyIndicator.IsBusy = true;
			await LoadDataAsync().ConfigureAwait(true);
			_busyIndicator.IsBusy = false;
		}

		private Task LoadDataAsync()
		{
			return Task.Factory.StartNew(LoadData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void LoadData()
		{
			var fileReader = new EncryptedVoteFileReader();

			var path=Assembly.GetExecutingAssembly().Location;
			var dir = System.IO.Path.GetDirectoryName(path);
			var ballotsFiles=Directory.GetFiles(dir, "ballots_encrypted_*");
			var voteRecords = fileReader.ReadFromFile(ballotsFiles[ballotsFiles.Length-1]);

			// время записи блока
			BlockTimestampInterpolator blockTimestampInterpolator = null;
			{
				var graphItems = BlockGraphBuilder.BlockStart(voteRecords, x => x.BlockNumber, x => x.Time);
				_blockTimestamp.ShowLinearIntepolatedBlockGraphItems(true, graphItems);
				blockTimestampInterpolator = new BlockTimestampInterpolator(graphItems);
				_blockTimestampGridView.LoadData(graphItems);
			}
			// среднее время вычисления блока
			{
				var graphItems = BlockGraphBuilder.BlockTime(voteRecords, x => x.BlockNumber, x => x.Time);
				_averageBlockTime.ShowLinearIntepolatedBlockGraphItems(false, graphItems);
				_blockTimeGridView.LoadData(graphItems);
			}	
			{
				var graphItems = BlockGraphBuilder.TransactionPerBlock(voteRecords,x=>true, x => x.BlockNumber);
				// транзакций в блоках
				_transactionPerBlock.Show(graphItems);
				// транзакций в блоках по времени
				_transactionPerBlockTimeline.Show(blockTimestampInterpolator, new[] { "Votes" }, new[] { graphItems });
			}
			// транзакций в блоках по времени по районам голосования
			{
				var districts = voteRecords.Select(x => x.DistrictNumber).Distinct().ToArray();
				var graphes = new BlockGraphItem<int>[districts.Length][];
				for (int i = 0; i < districts.Length; i++)
				{
					graphes[i] = BlockGraphBuilder.TransactionPerBlock(voteRecords, x => x.DistrictNumber == districts[i], x => x.BlockNumber);
				}
				var names = districts.Select(x =>
				{
					switch (x)
					{
						case 52:
							return "NN";
						case 77:
							return "MSK";
						default:
							return "";
					}
				}).ToArray();

				_transactionPerDistrictPerBlockTimeline.Show(blockTimestampInterpolator, names, graphes);
			}
		}
	}
}
