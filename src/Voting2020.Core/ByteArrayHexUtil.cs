using System;
using System.Runtime.CompilerServices;

namespace Voting2020.Core
{
	public static class ByteArrayToHexStringExtension
	{
		#region lowercase

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHexStringLowerCase(this byte[] array)
		{
			return ToHexStringLowerCase(array.AsSpan());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ToHexStringLowerCase(this ReadOnlySpan<byte> array)
		{
			string ret = new string('\0', array.Length * 2);
			fixed (char* strPtr = ret)
			{
				char* ptr = strPtr;
				for (int i = 0; i < array.Length; i++)
				{
					byte bytic = array[i];
					*ptr = HexChars.HexDigitsLowerCase[(bytic >> 4) & 0xF];
					ptr++;
					*ptr = HexChars.HexDigitsLowerCase[bytic & 0xF];
					ptr++;
				}
			}

			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHexStringLowerCase(this byte[] array, char separator)
		{
			return ToHexStringLowerCase(array.AsSpan(), separator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ToHexStringLowerCase(this ReadOnlySpan<byte> array, char separator)
		{
			string ret = new string('\0', array.Length * 3);

			byte bytic;
			int i = 0;
			fixed (char* strPtr = ret)
			{
				char* ptr = strPtr;
				for (; i < array.Length - 1; i++)
				{
					bytic = array[i];
					*ptr = HexChars.HexDigitsLowerCase[(bytic >> 4) & 0xF];
					ptr++;
					*ptr = HexChars.HexDigitsLowerCase[bytic & 0xF];
					ptr++;
					*ptr = separator;
					ptr++;
				}
				bytic = array[i];
				*ptr = HexChars.HexDigitsLowerCase[(bytic >> 4) & 0xF];
				ptr++;
				*ptr = HexChars.HexDigitsLowerCase[bytic & 0xF];
			}
			return ret;
		}
		#endregion

		#region uppercase

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHexStringUpperCase(this byte[] array)
		{
			return ToHexStringUpperCase(array.AsSpan());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ToHexStringUpperCase(this ReadOnlySpan<byte> array)
		{
			string ret = new string('\0', array.Length * 2);

			fixed (char* strPtr = ret)
			{
				char* ptr = strPtr;
				for (int i = 0; i < array.Length; i++)
				{
					byte bytic = array[i];
					*ptr = HexChars.HexDigitsUpperCaseLowerCase[(bytic >> 4) & 0xF];
					ptr++;
					*ptr = HexChars.HexDigitsUpperCaseLowerCase[bytic & 0xF];
					ptr++;
				}
			}
			return ret;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToHexStringUpperCase(this byte[] array, char separator)
		{
			return ToHexStringUpperCase(array.AsSpan(), separator);
		}

		public static unsafe string ToHexStringUpperCase(this ReadOnlySpan<byte> array, char separator)
		{
			string ret = new string('\0', array.Length * 3);

			byte bytic;
			int i = 0;
			fixed (char* strPtr = ret)
			{
				char* ptr = strPtr;
				for (; i < array.Length - 1; i++)
				{
					bytic = array[i];
					*ptr = HexChars.HexDigitsUpperCaseLowerCase[(bytic >> 4) & 0xF];
					ptr++;
					*ptr = HexChars.HexDigitsUpperCaseLowerCase[bytic & 0xF];
					ptr++;
					*ptr = separator;
					ptr++;
				}
				bytic = array[i];
				*ptr = HexChars.HexDigitsUpperCaseLowerCase[(bytic >> 4) & 0xF];
				ptr++;
				*ptr = HexChars.HexDigitsUpperCaseLowerCase[bytic & 0xF];
			}
			return ret;
		}
		#endregion
	}
}
