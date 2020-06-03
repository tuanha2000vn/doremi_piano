using UnityEngine;
using System;
using System.Collections;

using ForieroEngine.Extensions;

[ExecuteInEditMode]
public class DesignResolution : MonoBehaviour
{

	private static Vector2 _scaler = Vector2.one;
	#pragma warning disable 414
	private static float _width = 0;
	private static float _height = 0;
	#pragma warning restore 414
	public static Vector2 scaler {
		get {
#if !UNITY_EDITOR
			if (Screen.width != _width || Screen.height != _height) {
#endif
			_width = Screen.width;
			_height = Screen.height;
			_scaler = new Vector2 (
				((float)Screen.width / designResolution.x),
				((float)Screen.height / designResolution.y)
			);
			//Debug.Log ("SCALER : " + _scaler.ToString());
#if !UNITY_EDITOR			
			}
#endif
			return _scaler;
		}
	}

	public static Vector2 designResolution = new Vector2 (1600, 1200);

	public static Vector2 currentResolution = Vector2.zero;

	public static Action OnDisplayResolutionChange;

	void Awake ()
	{
		currentResolution = new Vector2 (Screen.width, Screen.height);
	}

	void Start ()
	{
		currentResolution = new Vector2 (Screen.width, Screen.height);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Screen.width != currentResolution.x || Screen.height != currentResolution.y) {
			currentResolution = new Vector2 (Screen.width, Screen.height);
			if (OnDisplayResolutionChange != null)
				OnDisplayResolutionChange ();
		}
	}

	public static Vector2 AspectRatio (int width, int height)
	{
		return new Vector2 (width / ForieroEngineExtensions.GreatestCommonDivider (width, height), height / ForieroEngineExtensions.GreatestCommonDivider (width, height));
	}

}
