using System;
using System.Linq;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Voting2020.Visualization
{
	public sealed class BlockTimePlotModelDrawer
		: BaseModelDrawer
	{
		private readonly PlotModel _plotModel;

		

		private bool _yIsTimeAxis;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="yIsTimeAxis">Является ли y осью времени (а не интервала)</param>
		public BlockTimePlotModelDrawer(bool yIsTimeAxis=false)
		{
			_yIsTimeAxis = yIsTimeAxis;
			_plotModel = new PlotModel();
			var xAxis = new LinearAxis()
			{
				Position = AxisPosition.Bottom,
				Key = XAxisKey,
				Title="Block Number"
			};
			_plotModel.Axes.Add(xAxis);

			var yAxis = new TimeSpanAxis()
			{
				Position = AxisPosition.Left,
				Key = YAxisKey,
				AbsoluteMinimum = TimeSpanAxis.ToDouble(TimeSpan.Zero),
				Minimum =  TimeSpanAxis.ToDouble(TimeSpan.Zero),
				Title="Time"
			};
			_plotModel.Axes.Add(yAxis);
		}

		public override PlotModel PlotModel
		{
			get { return _plotModel; }
		}

		private LineSeries PrepareSeries()
		{
			LineSeries series = null;
			if (_plotModel.Series.Count == 1)
			{
				series = (LineSeries) _plotModel.Series[0];
				series.Points.Clear();
			}
			else
			{
				_plotModel.Series.Clear();
				series = new LineSeries()
				{
					CanTrackerInterpolatePoints = false
				};
				_plotModel.Series.Add(series);
			}
			return series;
		}

		private void ConfigureAxes(bool setMinTime, BlockGraphItem<TimeSpan>[] array)
		{
			var xAxis = _plotModel.Axes.Where(x => x.Key == XAxisKey).Single();
			xAxis.SetAxisMinMax(array.Min(x => x.BlockNumber), array.Max(x => x.BlockNumber));
			var yAxis = _plotModel.Axes.Where(x => x.Key == "y_axis").Single();
			var maxTime = array.Max(x => x.Data);
			yAxis.SetAxisMax(maxTime);
			if (setMinTime)
			{
				var minTime = array.Min(x => x.Data);
				yAxis.SetAxisMin(minTime);
			}
			else
			{
				yAxis.SetAxisMin(TimeSpan.Zero);
			}
		}

		public void ShowLinearIntepolatedBlockGraphItems(bool setMinTime,BlockGraphItem<TimeSpan>[] array)
		{
			lock (_plotModel.SyncRoot)
			{
				ConfigureAxes(setMinTime, array);

				var series = PrepareSeries();
				for(int i = 0; i < array.Length; i++)
				{
					series.Points.Add(new DataPoint(array[i].BlockNumber, TimeSpanAxis.ToDouble(array[i].Data)));
				}
			}
			_plotModel.InvalidatePlot(true);
		}

		public void ShowLastConstValueInterpolatedBlockGraphItems(bool setMinTime, BlockGraphItem<TimeSpan>[] array)
		{
			lock (_plotModel.SyncRoot)
			{
				ConfigureAxes(setMinTime, array);

				var series = PrepareSeries();
				int lastBlock = array[0].BlockNumber;
				var lastBlockTimeValue = TimeSpanAxis.ToDouble(array[0].Data);
				series.Points.Add(new DataPoint(lastBlock, lastBlockTimeValue));


				for (int i = 1; i < array.Length; i++)
				{
					var prevAverageBlock = array[i].BlockNumber - 1;
					if (lastBlock < prevAverageBlock)
					{
						lastBlockTimeValue = TimeSpanAxis.ToDouble(array[i].Data);
						// расширяем значение на предыдущие блоки
						series.Points.Add(new DataPoint(lastBlock + 1, lastBlockTimeValue));

						// добавляем значение
						lastBlock = array[i].BlockNumber;
						series.Points.Add(new DataPoint(lastBlock, lastBlockTimeValue));
					}
					else
					{
						// просто добавляем это значение
						lastBlock = array[i].BlockNumber;
						lastBlockTimeValue = TimeSpanAxis.ToDouble(array[i].Data);
						series.Points.Add(new DataPoint(lastBlock, lastBlockTimeValue));
					}
				}
			}
			_plotModel.InvalidatePlot(true);
		}

		public string GetTimeAxisKey()
		{
			return _yIsTimeAxis ? YAxisKey : null;
		}

		public override string GetBlockNumberAxisKey()
		{
			return XAxisKey;
		}
	}
}
