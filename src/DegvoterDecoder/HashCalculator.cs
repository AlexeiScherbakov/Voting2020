using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

using Voting2020.Core;

namespace DegvoterDecoder
{
	public sealed class HashCalculator
		:IDisposable
	{
		private SHA256 _sha256= SHA256.Create();


		public HashCalculator()
		{

		}

		~HashCalculator()
		{
			Clear();
		}

		public void Dispose()
		{
			Clear();
			GC.SuppressFinalize(this);
		}

		
		private void Clear()
		{
			_sha256.Dispose();
		}


		public string ComputeHash(string data)
		{
			byte[] hash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
			return hash.ToHexStringLowerCase();
		}
	}
}
