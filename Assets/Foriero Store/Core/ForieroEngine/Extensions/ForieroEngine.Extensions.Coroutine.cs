using UnityEngine;
using System;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public class FireCoroutine
		{
			public readonly MonoBehaviour monoBehaviour;
			
			private bool paused = false;
			private bool killed = false;

			public FireCoroutine (MonoBehaviour m, int frameDelay, Action onFire)
			{
				monoBehaviour = m;
				m.StartCoroutine (FireFrameDelay (frameDelay, onFire));
			}

			public FireCoroutine (MonoBehaviour m, float delay, Action onFire)
			{
				monoBehaviour = m;
				m.StartCoroutine (FireFloatDelay (delay, onFire));
			}

			public void Pause ()
			{
				paused = true;
			}

			public void Continue ()
			{
				paused = false;
			}

			public void Kill ()
			{
				killed = true;	
			}

			IEnumerator FireFloatDelay (float delay, Action onFire)
			{
				while (delay > 0f) {
					if (killed)
						yield break;
					if (!paused)
						delay -= Time.deltaTime;
					yield return null;				
				}	
				onFire.Invoke ();
			}

			IEnumerator FireFrameDelay (int frameDelay, Action onFire)
			{
				while (frameDelay > 0) {
					if (killed)
						yield break;
					if (!paused)
						frameDelay--;
					yield return null;				
				}	
				onFire.Invoke ();
			}
		}

		public static FireCoroutine FireAction (this MonoBehaviour m, float delay, Action onFire)
		{
			return new FireCoroutine (m, delay, onFire);
		}

		public static FireCoroutine FireAction (this MonoBehaviour m, int frameDelay, Action onFire)
		{
			return new FireCoroutine (m, frameDelay, onFire);
		}
	}
}
