using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Voting2020.Core;
using Voting2020.Visualization;

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
			LoadEncryptedData();
		}

		private bool LoadEncryptedData()
		{
			var fileReader = new EncryptedVoteFileReader();

			var path = Assembly.GetExecutingAssembly().Location;
			var dir = System.IO.Path.GetDirectoryName(path);
			var ballotsFiles = Directory.GetFiles(dir, "ballots_encrypted_*");
			if (ballotsFiles.Length == 0)
			{
				Dispatcher.Invoke(delegate
				{
					MessageBox.Show(this, "Please add ballots_encrypted_* file to program directory");
				});
				return false;
			}

			var voteRecords = fileReader.ReadFromFile(ballotsFiles[ballotsFiles.Length - 1]);

			_voteDecryptorControl.LoadRecords(voteRecords);

			// время записи блока
			var blockStartGraphItems = BlockGraphBuilder.BlockStart(voteRecords, x => x.BlockNumber, x => x.Time);
			_blockTimestamp.ShowLinearIntepolatedBlockGraphItems(true, blockStartGraphItems);
			var blockTimestampInterpolator = new BlockTimestampInterpolator(blockStartGraphItems);
			_blockTimestampGridView.LoadData(blockStartGraphItems);

			// среднее время вычисления блока
			{
				var graphItems = BlockGraphBuilder.BlockTime(blockStartGraphItems);
				_averageBlockTime.ShowLinearIntepolatedBlockGraphItems(false, graphItems);
				_blockTimeGridView.LoadData(graphItems);
			}
			{
				var graphItems = BlockGraphBuilder.TransactionPerBlock(voteRecords, x => true, x => x.BlockNumber);
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
			return true;
		}
	}
}
