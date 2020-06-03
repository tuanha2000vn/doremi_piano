using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static Color R (this Color color, float r)
		{
			return new Color (r, color.g, color.b, color.a);
		}

		public static Color G (this Color color, float g)
		{
			return new Color (color.r, g, color.b, color.a);
		}

		public static Color B (this Color color, float b)
		{
			return new Color (color.r, color.g, b, color.a);
		}

		public static Color A (this Color color, float a)
		{
			return new Color (color.r, color.g, color.b, a);
		}

		//public static Color HEX(this Color,

		// Convert HEX to INT
		public static int HexToInt (this char hex)
		{
			switch (hex) {
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case 'A':
				return 10;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			case 'a':
				return 10;
			case 'b':
				return 11;
			case 'c':
				return 12;
			case 'd':
				return 13;
			case 'e':
				return 14;
			case 'f':
				return 15;
			}
			return 15;
		}

		public static string ToHex (this Color color)
		{
			Color32 c = (Color32)color;
			return c.r.ToString ("X2") + c.g.ToString ("X2") + c.b.ToString ("X2") + c.a.ToString ("X2"); 
		}

		public static Color HexToColor (string hexChars)
		{
			if (hexChars.Length == 7) {
				byte r = (byte)(hexChars [1].HexToInt () * 16 + hexChars [2].HexToInt ());
				byte g = (byte)(hexChars [3].HexToInt () * 16 + hexChars [4].HexToInt ());
				byte b = (byte)(hexChars [5].HexToInt () * 16 + hexChars [6].HexToInt ());
				
				return new Color32 (r, g, b, 255);
			} else {
				byte r = (byte)(hexChars [1].HexToInt () * 16 + hexChars [2].HexToInt ());
				byte g = (byte)(hexChars [3].HexToInt () * 16 + hexChars [4].HexToInt ());
				byte b = (byte)(hexChars [5].HexToInt () * 16 + hexChars [6].HexToInt ());
				byte a = (byte)(hexChars [7].HexToInt () * 16 + hexChars [8].HexToInt ());
				
				return new Color32 (r, g, b, a);
			}
		}
	}
}
