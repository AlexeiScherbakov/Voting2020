using System;

namespace Voting2020.Core
{
	public static class ByteArrayHexUtil
	{
		public static string ToHexString(this byte[] data)
		{
			return BitConverter.ToString(data).Replace("-", "");
		}
	}
}
