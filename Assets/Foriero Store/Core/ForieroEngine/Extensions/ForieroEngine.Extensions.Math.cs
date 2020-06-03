using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static int GreatestCommonDivider (this int a, int b)
		{
			int Remainder;

			while (b != 0) {
				Remainder = a % b;
				a = b;
				b = Remainder;
			}

			return a;
		}

		public static float Radians (this float value)
		{
			// value * (PI/180f) //
			return value * Mathf.Rad2Deg;
		}

		public static float Degrees (this float value)
		{
			// value * (180f/PI) //
			return value * Mathf.Rad2Deg;
		}

		public static float Cos (this float value)
		{
			return Mathf.Cos (value);	
		}

		public static float Sin (this float value)
		{
			return Mathf.Sin (value);	
		}

		public static float Tan (this float value)
		{
			return Mathf.Tan (value);	
		}
	}
}
