using System;
using System.Collections.Generic;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using Voting2020.Visualization;

namespace Voting2019.Visualization
{
	public sealed class TransactionPerBlockModelDrawer
		: BaseModelDrawer
	{
		private readonly PlotModel _plotModel;
		private readonly LinearAxis _xAxis;
		private readonly LinearAxis _yAxis;


		public TransactionPerBlockModelDrawer()
		{
			_plotModel = new PlotModel();
			_xAxis = new LinearAxis()
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


		private void ConfigureAxes(BlockGraphItem<int>[] array)
		{
			var xAxis = _plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
			xAxis.SetAxisMinMax(array.Min(x => x.BlockNumber), array.Max(x => x.BlockNumber));
			var yAxis = _plotModel.Axes.Where(x => x.Key == YAxisKey).Single();
			yAxis.SetAxisMin(0);
		}

		public void Show(BlockGraphItem<int>[] array)
		{
			lock (_plotModel.SyncRoot)
			{
				ConfigureAxes(array);

				_plotModel.Series.Clear();

				var series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false
				};

				_plotModel.Series.Add(series);

				ref var lastPoint = ref array[0];
				series.Points.Add(new DataPoint(lastPoint.BlockNumber, lastPoint.Data));
				int maxVotes = lastPoint.Data;
				for (int i = 1; i < array.Length; i++)
				{
					ref var currentPoint = ref array[i];
					for (int point = lastPoint.BlockNumber + 1; point < currentPoint.BlockNumber; point++)
					{
						series.Points.Add(new DataPoint(point, 0));
					}
					if (currentPoint.Data > maxVotes)
					{
						maxVotes = currentPoint.Data;
					}
					series.Points.Add(new DataPoint(currentPoint.BlockNumber, currentPoint.Data));
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
