using System;
using OxyPlot.Axes;

using Voting2020.Core;

namespace Voting2020.Visualization
{
	internal static class AxisHelper
	{
		public static void SetAxisMin(this Axis axis, double min)
		{
			axis.AbsoluteMinimum = axis.Minimum = min;
		}

		public static void SetAxisMin(this Axis axis, TimeSpan min)
		{
			axis.AbsoluteMinimum = axis.Minimum = TimeSpanAxis.ToDouble(min);
		}

		public static void SetAxisMax(this Axis axis, double max)
		{
			axis.AbsoluteMaximum = axis.Maximum = max;
		}

		public static void SetAxisMax(this Axis axis, TimeSpan max)
		{
			axis.AbsoluteMaximum = axis.Maximum = TimeSpanAxis.ToDouble(max);
		}

		public static void SetAxisMinMax(this Axis axis, double min, double max)
		{
			SetAxisMin(axis, min);
			SetAxisMax(axis, max);
		}

		public static void SetMinMaxBlocksToXAxis(this Axis axis, IBlockChainBlockRange blockChainBlockRange)
		{
			SetAxisMinMax(axis, blockChainBlockRange.MinBlockNumber, blockChainBlockRange.MaxBlockNumber);
		}

		public static void SetAxisMinMax(this TimeSpanAxis axis, TimeSpan min, TimeSpan max)
		{
			SetAxisMin(axis, min);
			SetAxisMax(axis, max);
		}

		
	}
}
