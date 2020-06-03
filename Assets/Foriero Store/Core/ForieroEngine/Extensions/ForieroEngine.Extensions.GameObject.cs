using UnityEngine;
using System.Collections.Generic;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		//Defined in the common base class for all mono behaviours
		public static I GetInterfaceComponent<I> (this GameObject o) where I : class
		{
			return o.GetComponent (typeof(I)) as I;
		}

		public static List<I> FindObjectsOfInterface<I> (this GameObject o) where I : class
		{
			MonoBehaviour[] monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour> ();
			List<I> list = new List<I> ();
			
			foreach (MonoBehaviour behaviour in monoBehaviours) {
				I component = behaviour.GetComponent (typeof(I)) as I;
				
				if (component != null) {
					list.Add (component);
				}
			}
			
			return list;
		}

		public static T GetSafeComponent<T> (this GameObject obj) where T : MonoBehaviour
		{
			T component = obj.GetComponent<T> ();
			
			if (component == null) {
				Debug.LogError ("Expected to find component of type "
				+ typeof(T) + " but found none", obj);
			}
			
			return component;
		}
	}
}
