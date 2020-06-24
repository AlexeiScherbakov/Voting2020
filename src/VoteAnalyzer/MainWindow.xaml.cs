using System;
using System.Collections.Generic;
using System.Linq;
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
using Voting2019.Visualization;
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

		public MainWindow()
		{
			InitializeComponent();

			_blockTimestamp = new BlockTimePlotModelDrawer(true);
			_averageBlockTime = new BlockTimePlotModelDrawer(false);
			_transactionPerBlock = new TransactionPerBlockModelDrawer();
			_transactionPerBlockTimeline = new TransactionPerBlockTimelineModelDrawer();

			_blockTimestampPlotView.Model = _blockTimestamp.PlotModel;
			_blockTimePlotView.Model = _averageBlockTime.PlotModel;
			_transactionPerBlockPlotView.Model = _transactionPerBlock.PlotModel;
			_transactionPerBlockTimelinePlotView.Model = _transactionPerBlockTimeline.PlotModel;

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
			var voteRecords = fileReader.ReadFromFile("ballots_encrypted_2020-06-19T17_00_00.csv");

			// время записи блока
			BlockTimestampInterpolator blockTimestampInterpolator = null;
			{
				var graphItems = BlockGraphBuilder.BlockStart(voteRecords, x => x.BlockNumber, x => x.Time);
				_blockTimestamp.ShowLinearIntepolatedBlockGraphItems(true, graphItems);
				blockTimestampInterpolator = new BlockTimestampInterpolator(graphItems);

				Dispatcher.Invoke(delegate
				{
					_blockTimestampGridView.ItemsSource = graphItems;
				});
			}
			// среднее время вычисления блока
			{
				var graphItems = BlockGraphBuilder.BlockTime(voteRecords, x => x.BlockNumber, x => x.Time);
				_averageBlockTime.ShowLinearIntepolatedBlockGraphItems(false, graphItems);
			}
			{
				var graphItems = BlockGraphBuilder.TransactionPerBlock(voteRecords, x => x.BlockNumber);
				_transactionPerBlock.Show(graphItems);
			}
			{
				var graphItems = BlockGraphBuilder.TransactionPerBlock(voteRecords, x => x.BlockNumber);
				_transactionPerBlockTimeline.Show(graphItems, blockTimestampInterpolator);
			}
		}
	}
}
