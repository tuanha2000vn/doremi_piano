using UnityEngine;
using System.Collections;
using System;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		//		public static T SetFlag <T> (this T a, T b) where T : struct, IConvertible
		//		{
		//			if (!typeof(T).IsEnum) {
		//				Debug.LogError ("T must be an enumerated type");
		//			}
		//
		//			return a | b;
		//		}
		//
		//		public static T UnsetFlag <T> (this T a, T b) where T : struct, IConvertible
		//		{
		//			if (!typeof(T).IsEnum) {
		//				Debug.LogError ("T must be an enumerated type");
		//			}
		//
		//			return a & (~b);
		//		}

		// works well with 'none'
		public static bool HasFlag <T> (this T a, T b) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum()) {
				Debug.LogError ("T must be an enumerated type");
			}

			var aValue = Convert.ToUInt64 (a);
			var bValue = Convert.ToUInt64 (b);

			return (aValue & bValue) == bValue;
		}

		//		public static T ToogleFlag <T> (this T a, T b) where T : struct, IConvertible
		//		{
		//			if (!typeof(T).IsEnum) {
		//				Debug.LogError ("T must be an enumerated type");
		//			}
		//
		//			var aValue = Convert.ToUInt64 (a);
		//			var bValue = Convert.ToUInt64 (b);
		//
		//			return (aValue ^ bValue);
		//		}
	}
}
