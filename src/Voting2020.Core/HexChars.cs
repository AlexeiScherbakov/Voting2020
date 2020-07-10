namespace Voting2020.Core
{
	internal static class HexChars
	{
		/// <summary>
		/// Hex digit bytes in ASCII order (only lowercase)
		/// </summary>
		internal static readonly char[] HexDigitsLowerCase = new char[]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f'
		};

		/// <summary>
		/// Hex digits chars in ASCII order (uppercase first)
		/// </summary>
		internal static readonly char[] HexDigitsUpperCaseLowerCase = new char[]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F',
			'a', 'b', 'c', 'd', 'e', 'f'
		};


		internal static readonly int[] PairHexDigitsLowerCase;
		internal static readonly int[] PairHexDigitsUpperCase;


		static HexChars()
		{
			PairHexDigitsLowerCase = new int[256];
			PairHexDigitsUpperCase = new int[256];
			for (int i = 0; i < 256; i++)
			{
				PairHexDigitsLowerCase[i] = HexDigitsLowerCase[i & 0x0F] << 16 | HexDigitsLowerCase[(i & 0xF) >> 4];
				PairHexDigitsUpperCase[i] = HexDigitsUpperCaseLowerCase[i & 0x0F] << 16 | HexDigitsUpperCaseLowerCase[(i & 0xF) >> 4];
			}
		}
	}
}
