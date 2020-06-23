using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2020.Core;
using Voting2020.Visualization;

namespace Voting2019.Visualization
{
	public sealed class TransactionPerBlockTimelineModelDrawer
		: BaseModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly TimeSpanAxis _xAxis;
		private readonly LinearAxis _yAxis;


		public TransactionPerBlockTimelineModelDrawer()
		{
			_plotModel = new PlotModel();
			_xAxis = new TimeSpanAxis()
			{
				Position = AxisPosition.Bottom,
				Key = XAxisKey
			};
			_plotModel.Axes.Add(_xAxis);
			_yAxis = new LinearAxis()
			{
				Position = AxisPosition.Left,
				Key = YAxisKey,
				AbsoluteMinimum = 0,
				AbsoluteMaximum = 0
			};
			_plotModel.Axes.Add(_yAxis);
		}


		public PlotModel PlotModel
		{
			get { return _plotModel; }
		}


		private void ConfigureAxes(BlockGraphItem<int>[] array, IBlockTimestampInterpolator blockTimestampInterpolator)
		{
			var xAxis = (TimeSpanAxis) _plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
			xAxis.SetAxisMinMax(
				blockTimestampInterpolator.GetBlockTimestamp(array.Min(x => x.BlockNumber)),
				blockTimestampInterpolator.GetBlockTimestamp(array.Max(x => x.BlockNumber)));
			var yAxis = _plotModel.Axes.Where(x => x.Key == YAxisKey).Single();
			yAxis.SetAxisMin(0);
		}

		public void Show(BlockGraphItem<int>[] array, IBlockTimestampInterpolator blockTimestampInterpolator)
		{
			lock (_plotModel.SyncRoot)
			{
				ConfigureAxes(array, blockTimestampInterpolator);

				_plotModel.Series.Clear();

				var series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false
				};

				_plotModel.Series.Add(series);

				ref var lastPoint = ref array[0];
				series.Points.Add(new DataPoint(
					TimeSpanAxis.ToDouble(blockTimestampInterpolator.GetBlockTimestamp(lastPoint.BlockNumber)),
					lastPoint.Data));

				int maxVotes = lastPoint.Data;
				for (int i = 1; i < array.Length; i++)
				{
					ref var currentPoint = ref array[i];
					for (int point = lastPoint.BlockNumber + 1; point < currentPoint.BlockNumber; point++)
					{
						var xPoint = TimeSpanAxis.ToDouble(blockTimestampInterpolator.GetBlockTimestamp(point));
						series.Points.Add(new DataPoint(xPoint, 0));
					}
					if (currentPoint.Data > maxVotes)
					{
						maxVotes = currentPoint.Data;
					}
					series.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(blockTimestampInterpolator.GetBlockTimestamp(currentPoint.BlockNumber)), currentPoint.Data));
					lastPoint = currentPoint;
				}
				_yAxis.SetAxisMax(maxVotes);
			}
			_plotModel.InvalidatePlot(true);
		}

		public string GetTimeAxisKey()
		{
			return null;
		}

		public override string GetBlockNumberAxisKey()
		{
			return XAxisKey;
		}
	}
}
