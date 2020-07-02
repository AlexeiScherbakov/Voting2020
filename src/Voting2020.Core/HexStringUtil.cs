using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Voting2020.Core
{
	public static class HexStringUtil
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int HexCharToInt(char ch)
		{
			return ((ch & 0xFF80) == 0) ? HexLookupTable.CharCodeToInt[ch] : -1;
		}

		public static int ParseInt32FromHexString(this ReadOnlySpan<char> str, int start, out int value)
		{
			Contract.Requires(str != null);
			Contract.Requires(str.Length > start);

			value = 0;
			int ret = 0;
			for (int i = start; i < str.Length; i++)
			{
				int pos = HexCharToInt(str[i]);
				if (pos < 0)
				{
					break;
				}
				value <<= 4;
				value |= pos;
				ret++;
			}
			return ret;
		}

		/// <summary>
		/// Parses byte array from hex string
		/// </summary>
		/// <param name="input">Input string</param>
		/// <param name="start">Start position to parsing</param>
		/// <param name="array">Output buffer</param>
		/// <returns></returns>
		public static bool TryParseByteArrayFromHexString(this ReadOnlySpan<char> input, Span<byte> array)
		{
			int end = array.Length * 2;
			int position = 0;
			for (int i = 0; i < end; i += 2)
			{
				int current = 0;
				int pos = HexCharToInt(input[i]);
				if (pos < 0) return false;
				current = pos;
				pos = HexCharToInt(input[i + 1]);
				if (pos < 0) return false;
				current <<= 4;
				current |= pos;
				array[position] = (byte) current;
				position++;
			}
			return true;
		}

		public static bool TryParseByteArrayFromHexString(this string input, Span<byte> array)
		{
			return TryParseByteArrayFromHexString(input.AsSpan(), array);
		}

		public static bool TryParseByteArrayFromHexString(this ReadOnlySpan<char> input, out byte[] output)
		{
			Contract.Requires(input != null);
			Contract.Requires(input.Length % 2 == 0);

			int byteCount = input.Length / 2;
			byte[] retArray = new byte[byteCount];
			bool ret;
			if (ret = TryParseByteArrayFromHexString(input, retArray))
			{
				output = retArray;
			}
			else
			{
				output = null;
			}
			return ret;
		}

		public static bool TryParseByteArrayFromHexString(this string input, out byte[] output)
		{
			return TryParseByteArrayFromHexString(input.AsSpan(), out output);
		}

		public static byte[] ParseByteArrayFromHexString(this ReadOnlySpan<char> input)
		{
			Contract.Requires(input != null);
			Contract.Requires(input.Length % 2 == 0);

			byte[] ret;
			return TryParseByteArrayFromHexString(input, out ret) ? ret : null;
		}

		public static byte[] ParseByteArrayFromHexString(this string input)
		{
			Contract.Requires(input != null);
			Contract.Requires(input.Length % 2 == 0);

			byte[] ret;
			return TryParseByteArrayFromHexString(input, out ret) ? ret : null;
		}

		public static byte[] ParseHexEncodedValue(this string input)
		{
			ReadOnlySpan<char> val = input;
			if (val.StartsWith("0x"))
			{
				val = val.Slice(2);
			}
			return val.ParseByteArrayFromHexString();
		}
	}
}
