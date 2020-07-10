using System.Collections.Generic;
using System.Threading;

namespace DegvoterDecoder
{
	public sealed class HashBruteforcer
	{
		private Thread[] _threads;
		private BruteforceState _state;
		private IResultLog _log;
		private HashSet<string> _hashes;

		public HashBruteforcer(HashSet<string> hashes, int threadCount,BruteforceState state, IResultLog log)
		{
			_threads = new Thread[threadCount];
			_state = state;
			_hashes = hashes;
			_log = log;
			for (int i = 0; i < _threads.Length; i++)
			{
				_threads[i] = new Thread(BruteForceThread);
				_threads[i].Start(i);
			}
		}



		private void BruteForceThread(object num)
		{
			int serie;
			using var hasher = new HashCalculator();

			while ((serie = _state.TakeSerie()) >= 0)
			{
				for (int i = 0; i < 1000000; i++)
				{
					string passport = string.Format("{0:D4}{1:D6}", serie, i);
					var hash = hasher.ComputeHash(passport);
					if (_hashes.Contains(hash))
					{
						_log.InformFound(serie, i);
					}
				}
				_state.InformCompleted(serie);
			}
		}


		public void Wait()
		{
			foreach(var thread in _threads)
			{
				thread.Join();
			}
		}
	}
}
