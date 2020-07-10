using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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

using OxyPlot;

using Voting2020.Core;

namespace DegvoterDecoder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HashSet<string> _passportHashes = new HashSet<string>();

		private PlotModel _plotModel = new PlotModel();

		private BruteforceState _state;

		public MainWindow()
		{
			InitializeComponent();

			_plotView.Model = _plotModel;


			this.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			
			string connectionString = string.Format("Data Source=" + System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\db.sqlite");
			using var sqliteConnection = new SQLiteConnection(connectionString);
			sqliteConnection.Open();
			using var cmd = sqliteConnection.CreateCommand();
			cmd.CommandText = "SELECT num,used FROM passports";

			using var reader = cmd.ExecuteReader();
			var c1=reader.GetOrdinal("num");
			var c2 = reader.GetOrdinal("used");

			

			while (reader.Read())
			{
				string hash = reader.GetString(c1);
				bool used = reader.GetBoolean(c2);
				_passportHashes.Add(hash);
			}

			Task.Factory.StartNew(() => Test(), TaskCreationOptions.LongRunning);
		}

		
		private void Test()
		{
			_state = new BruteforceState("program.state");
			var resultLog = new FileResultLog("results.txt");
			HashBruteforcer hashBruteforcer = new HashBruteforcer(_passportHashes, 8, _state, resultLog);

			hashBruteforcer.Wait();

			resultLog.Dispose();

			using var streamReader = File.OpenText("results.txt");
			Dictionary<int, int> counters = new Dictionary<int, int>();
			while (!streamReader.EndOfStream)
			{
				ReadOnlySpan<char> line = streamReader.ReadLine();
				var series= line.Slice(0, 4);
				if (int.TryParse(series,out var intSeries))
				{
					if (!counters.TryGetValue(intSeries,out int counter))
					{
						counters.Add(intSeries, 1);
					}
					else
					{
						counters[intSeries] = counter + 1;
					}
				}
			}
			var lineSeries = new OxyPlot.Series.LineSeries()
			{
				CanTrackerInterpolatePoints = false
			};
			foreach(var pair in counters)
			{
				lineSeries.Points.Add(new OxyPlot.DataPoint(pair.Key, pair.Value));
			}
			lineSeries.Points.Sort((x, y) => Math.Sign(x.X - y.X));


			_plotModel.Series.Add(lineSeries);

			_plotModel.InvalidatePlot(true);


		}
	}
}
