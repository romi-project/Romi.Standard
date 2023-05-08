using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Romi.Standard
{
public static class Locales
	{
		public static readonly Encoding ASCII = Encoding.ASCII;
		public static readonly Encoding Unicode;
		public static readonly Encoding UTF8 = Encoding.UTF8;
		public static readonly Encoding Codepage949;

		public static long TimeOffset => TimeZoneInfo.Local.BaseUtcOffset.Ticks;

		static Locales()
		{
			Codepage949 = Encoding.GetEncoding(949, EncoderFallback.ExceptionFallback, DecoderFallback.ReplacementFallback);
			Unicode = Encoding.GetEncoding(1200, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback);
		}

		public static string GetTextWithPP(string text, PPType type)
		{
			return text + GetKoreanPP(text, type);
		}

		public static string GetKoreanPP(string text, PPType type)
		{
			text = text.Trim();
			var lastChar = text[text.Length - 1];
			int jongsung;
			if (lastChar >= 0xAC00 && lastChar < 0xD800)
			{
				jongsung = (lastChar - 0xAC00) % 28;
			}
			else
			{
				switch (lastChar)
				{
					case '0':
					case '1':
					case '3':
					case '6':
					case '7':
					case '8':
					case 'l':
					case 'm':
					case 'n':
					case 'r':
						jongsung = 1;
						break;
					case '2':
					case '4':
					case '5':
					case '9':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
					case 'g':
					case 'h':
					case 'i':
					case 'j':
					case 'k':
					case 'o':
					case 'p':
					case 'q':
					case 's':
					case 't':
					case 'u':
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case 'z':
						jongsung = 0;
						break;
					default:
						jongsung = -1;
						break;
				}
			}
			if (jongsung == 0)
			{
				switch (type)
				{
					case PPType.이가:
						return "가";
					case PPType.은는:
						return "는";
					case PPType.을를:
						return "를";
					case PPType.과와:
						return "와";
				}
			}
			else if (jongsung > 0)
			{
				switch (type)
				{
					case PPType.이가:
						return "이";
					case PPType.은는:
						return "은";
					case PPType.을를:
						return "을";
					case PPType.과와:
						return "과";
				}
			}
			return "";
		}

		public static string EncodeString(byte[] value, int eLength = 0, Encoding encoding = null)
		{
			int length;
			if (eLength == 0)
			{
				var nullIndex = Array.IndexOf(value, (byte)0);
				length = nullIndex != -1 ? nullIndex : value.Length;
			}
			else
			{
				length = eLength;
			}
			if (encoding == null)
				encoding = UTF8;
			return encoding.GetString(value, 0, length);
		}

		public static byte[] DecodeString(string value, int len = 0, Encoding encoding = null)
		{
			if (encoding == null)
				encoding = UTF8;
			var bytes = encoding.GetBytes(value);
			if (len == 0)
			{
				return bytes;
			}
			else
			{
				var result = new byte[len];
				Array.Copy(bytes, result, Math.Min(bytes.Length, result.Length));
				return result;
			}
		}

		public static bool IsAllAscii(IEnumerable<byte> data)
		{
			return data.All(b => b >= 0x21 && b <= 0x7e);
		}

		public static bool IsInLength(string value, int min, int max)
		{
			var bytes = DecodeString(value);
			return bytes.Length >= min && bytes.Length <= max;
		}
	}
}
