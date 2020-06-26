using System;
using System.Diagnostics.Contracts;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2020.Core;

namespace Voting2020.Visualization
{
	public sealed class TransactionPerBlockTimelineModelDrawer
		: BaseModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly TimeSpanAxis _xAxis;


		public TransactionPerBlockTimelineModelDrawer()
		{
			_plotModel = new PlotModel();
			_xAxis = new TimeSpanAxis()
			{
				Position = AxisPosition.Bottom,
				Key = XAxisKey,
				MinorGridlineStyle= LineStyle.Dash,
				MajorGridlineStyle = LineStyle.Solid,
				MinorStep = TimeSpanAxis.ToDouble(TimeSpan.FromHours(1)),
				Title="Time"
			};
			_plotModel.Axes.Add(_xAxis);
		}


		public override PlotModel PlotModel
		{
			get { return _plotModel; }
		}


		private void ConfigureAxes(IBlockTimestampInterpolator blockTimestampInterpolator,BlockGraphItem<int>[][] array)
		{
			var xAxis = (TimeSpanAxis) _plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
			xAxis.SetAxisMinMax(
				blockTimestampInterpolator.GetBlockTimestamp(array.Min(x => x.Min(x => x.BlockNumber))),
				blockTimestampInterpolator.GetBlockTimestamp(array.Max(x => x.Max(x => x.BlockNumber))));
		}

		public void Show(IBlockTimestampInterpolator blockTimestampInterpolator,string[] names, BlockGraphItem<int>[][] array)
		{
			Contract.Requires(names.Length == array.Length);

			lock (_plotModel.SyncRoot)
			{
				ConfigureAxes(blockTimestampInterpolator, array);

				_plotModel.Series.Clear();

				const double GapSize = 0.05;
				int AxisCount = names.Length;
				double size = (1 - GapSize * (AxisCount - 1)) / AxisCount;
				double start = 0;

				for (int axisCounter = 0;axisCounter< names.Length; axisCounter++)
				{
					var data = array[axisCounter];

					var yAxisKey = Guid.NewGuid().ToString();
					var yAxis = new LinearAxis()
					{
						Position = AxisPosition.Left,
						Key = yAxisKey,
						StartPosition = start,
						EndPosition = start + size,
						Title=names[axisCounter]
					};
					start += size + GapSize;

					_plotModel.Axes.Add(yAxis);
					var series = new LineSeries()
					{
						CanTrackerInterpolatePoints = false,
						YAxisKey = yAxisKey
					};

					_plotModel.Series.Add(series);

					ref var lastPoint = ref data[0];
					series.Points.Add(new DataPoint(
						TimeSpanAxis.ToDouble(blockTimestampInterpolator.GetBlockTimestamp(lastPoint.BlockNumber)),
						lastPoint.Data));

					int maxVotes = lastPoint.Data;
					for (int i = 1; i < data.Length; i++)
					{
						ref var currentPoint = ref data[i];
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
					yAxis.SetAxisMin(0);
					yAxis.SetAxisMax(maxVotes);
				}
				
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
