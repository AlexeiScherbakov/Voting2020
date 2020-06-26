using OxyPlot;

namespace Voting2020.Visualization
{
	public abstract class BaseModelDrawer
	{
		protected const string XAxisKey = "x_axis";
		protected const string YAxisKey = "y_axis";


		public abstract PlotModel PlotModel { get; }

		/// <summary>
		/// Возвращает шкалу с номером блока
		/// </summary>
		/// <returns></returns>
		public abstract string GetBlockNumberAxisKey();
	}
}
