using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using OxyPlot;
using OxyPlot.Wpf;

using Voting2020.Visualization;

namespace VoteAnalyzer.Controls
{
	/// <summary>
	/// Interaction logic for BlockGraphItemPlotView.xaml
	/// </summary>
	public partial class BlockGraphItemPlotView
		: UserControl
	{
		public BlockGraphItemPlotView()
		{
			InitializeComponent();
		}


		public void LoadModelDrawer(BaseModelDrawer modelDrawer)
		{
			_plotView.Model = modelDrawer.PlotModel;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var s = new SaveFileDialog();
			s.Filter = "*.png|*.png";
			if (s.ShowDialog() != true)
			{
				return;
			}

			PngExporter.Export(_plotView.Model, s.FileName, 1920, 1080, OxyColors.White);
		}
	}
}
