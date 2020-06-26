using System;
using System.Collections.Generic;
using System.IO;
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

using OfficeOpenXml;

using Voting2020.Visualization;

namespace VoteAnalyzer.Controls
{
	/// <summary>
	/// Interaction logic for BlockGraphItemTableView.xaml
	/// </summary>
	public partial class BlockGraphItemTableView
		: UserControl
	{
		private ExcelExporter _excelExporter;

		public BlockGraphItemTableView()
		{
			InitializeComponent();
		}

		private void ExportToExcelClick(object sender, RoutedEventArgs args)
		{
			SaveFileDialog s = new SaveFileDialog();
			if (s.ShowDialog() != true) return;

			try
			{
				_excelExporter.ExportToExcelFile(s.FileName);
			}
			catch (Exception e)
			{

			}

		}

		public void LoadData<T>(BlockGraphItem<T>[] graphItems)
			where T : struct, IEquatable<T>
		{
			if (Dispatcher.CheckAccess())
			{
				_gridView.ItemsSource = graphItems;
			}
			else
			{
				Dispatcher.Invoke(delegate
				{
					_gridView.ItemsSource = graphItems;
				});
			}
			_excelExporter = new ExcelExporter<T>(graphItems, x => x.ToString());
		}


		public abstract class ExcelExporter
		{
			public abstract void ExportToExcelFile(string fileName);
		}

		private sealed class ExcelExporter<T>
			: ExcelExporter
			where T : struct, IEquatable<T>
		{
			BlockGraphItem<T>[] _graphItems;
			private readonly Func<T, string> _formatter;

			public ExcelExporter(BlockGraphItem<T>[] graphItems, Func<T, string> formatter = null)
			{
				_graphItems = graphItems;
				_formatter = formatter;
			}

			public override void ExportToExcelFile(string fileName)
			{
				ExcelPackage package = new ExcelPackage();

				var worksheet = package.Workbook.Worksheets.Add("Data export");
				for (int i = 0; i < _graphItems.Length; i++)
				{
					worksheet.Cells[1 + i, 1].Value = _graphItems[i].BlockNumber;
					if (_formatter is null)
					{
						worksheet.Cells[1 + i, 2].Value = _graphItems[i].Data;
					}
					else
					{
						worksheet.Cells[1 + i, 2].Value = _formatter(_graphItems[i].Data);
					}

				}

				package.SaveAs(new System.IO.FileInfo(fileName));
			}


		}
	}
}
