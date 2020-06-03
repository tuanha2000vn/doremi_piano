using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static float Distance (this float f1, float f2)
		{
			return Mathf.Sqrt (Mathf.Pow (f2 - f1, 2));
		}
	}
}
