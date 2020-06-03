using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		#if UNITY_IPHONE
		public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(this Dictionary<TKey, TValue> source){
			System.Random r = new System.Random();
			Debug.LogError("Please use instead Shuffle -> OrderBy(x => r.Next()).ToDictionary(item => item.Key, item => item.Value)");
			return source.OrderBy(x => r.Next()).ToDictionary(item => item.Key, item => item.Value);
		}
		
		public static Dictionary<TKey, TValue> SortByValue<TKey, TValue>(this Dictionary<TKey, TValue> source){
			Debug.LogError("Please use instead SortByValue -> OrderBy(x => x.Value)	.ToDictionary(item => item.Key, item => item.Value)");
			return source.OrderBy(x => x.Value)	.ToDictionary(item => item.Key, item => item.Value);
		}
		
		public static Dictionary<TKey, TValue> SortByKey<TKey, TValue>(this Dictionary<TKey, TValue> source){
			Debug.LogError("Please use instead SortByKey -> OrderBy(x => x.Key).ToDictionary(item => item.Key, item => item.Value)");
			return source.OrderBy(x => x.Key).ToDictionary(item => item.Key, item => item.Value);
		}



#else
		public static Dictionary<TKey, TValue> Shuffle<TKey, TValue> (this Dictionary<TKey, TValue> source)
		{
			System.Random r = new System.Random ();
			return source.OrderBy (x => r.Next ()).ToDictionary (item => item.Key, item => item.Value);
		}

		public static Dictionary<TKey, TValue> SortByValue<TKey, TValue> (this Dictionary<TKey, TValue> source)
		{
			return source.OrderBy (x => x.Value).ToDictionary (item => item.Key, item => item.Value);
		}

		public static Dictionary<TKey, TValue> SortByKey<TKey, TValue> (this Dictionary<TKey, TValue> source)
		{
			return source.OrderBy (x => x.Key)
				.ToDictionary (item => item.Key, item => item.Value);
		}
		#endif

		public static void Shuffle<T> (this IList<T> list)
		{  
			System.Random rng = new System.Random ();  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next (n + 1);  
				T value = list [k];  
				list [k] = list [n];  
				list [n] = value;  
			}  
		}
	}
}
