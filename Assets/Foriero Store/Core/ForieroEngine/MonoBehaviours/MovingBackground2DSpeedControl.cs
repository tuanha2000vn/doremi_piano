using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class MovingBackground2DSpeedControl : MonoBehaviour
{
		public Slider slider;

		MovingBackground2D[] backgrounds = new MovingBackground2D[0];

		void Awake ()
		{
				backgrounds = GameObject.FindObjectsOfType<MovingBackground2D> ();
		}

		public void OnSliderChange ()
		{
				foreach (MovingBackground2D b in backgrounds) {
						b.timeScale = -slider.value;
				}
		}
}
