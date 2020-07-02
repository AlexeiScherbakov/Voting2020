namespace Voting2020.Core
{
	internal static class HexLookupTable
	{
		/// <summary>
		/// Перевод из кода символа в значение
		/// '0' - 0
		/// 'f' и 'F' - 15
		/// </summary>
		internal static readonly int[] CharCodeToInt;

		static HexLookupTable()
		{
			CharCodeToInt = new int[128];
			for (int i = 0; i < 128; i++)
			{
				CharCodeToInt[i] = ConvertHexCharToInt((char) i);
			}
		}

		private static int ConvertHexCharToInt(char ch)
		{
			// there in worst case we have 4 compares
			const int LowerCaseShift = 'a' - 10;
			const int UpperCaseShift = 'A' - 10;

			return (ch >= 'A')
				? ((ch <= 'F') ? (ch - UpperCaseShift) : (((ch < 'a') || (ch > 'f')) ? -1 : (ch - LowerCaseShift)))
				: (((ch < '0') || (ch > '9')) ? -1 : (ch - '0'));
		}
	}
}
