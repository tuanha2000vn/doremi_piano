using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ScreenPlacement : MonoBehaviour
{
	
	public Camera cam;
	public TextAnchor anchor;
	public Vector2 pixelOffset;
		
	public bool useResRatio = true;
	public bool continuous = true;
	public bool onResolutionChange = true;
	public bool inEditor = true;

	Vector2 resolution = Vector2.zero;

	TextAnchor lastAnchor = TextAnchor.MiddleCenter;
	Vector2 lastPixelOffset = Vector2.zero;
	Camera lastCam = null;
	bool lastUseResRation = false;
	bool lastContinuous = false;
	bool lastOnResolutionChange = false;
	bool lastInEditor = false;

	void Awake ()
	{
		if (!cam) {
			cam = Camera.main;
		}

		resolution = new Vector2 (Screen.width, Screen.height);

		lastAnchor = anchor;
		lastPixelOffset = pixelOffset;
		lastCam = cam;
		lastUseResRation = useResRatio;
		lastContinuous = continuous;
		lastOnResolutionChange = onResolutionChange;
		lastInEditor = inEditor;

		ApplyScreenPlacement ();
	}

	void Start ()
	{
		resolution = new Vector2 (Screen.width, Screen.height);
		ApplyScreenPlacement ();
	}

	void LateUpdate ()
	{
		if (Application.isEditor && !Application.isPlaying) {
			if (inEditor) {
				ApplyEditor ();
			}
		} else {
			Apply ();
		}
	}

	void ApplyEditor ()
	{
		bool changed = lastAnchor != anchor
		               || lastPixelOffset.x != pixelOffset.x
		               || lastPixelOffset.y != pixelOffset.y
		               || lastCam != cam
		               || lastUseResRation != useResRatio
		               || lastContinuous != continuous
		               || lastOnResolutionChange != onResolutionChange
		               || lastInEditor != inEditor
		               || Screen.width != resolution.x
		               || Screen.height != resolution.y;

		if (changed) {
			resolution = new Vector2 (Screen.width, Screen.height);
			ApplyScreenPlacement ();
		}
	}

	void Apply ()
	{
		if (continuous) {
			if (onResolutionChange) {
				if (Screen.width != resolution.x || Screen.height != resolution.y) {
					resolution = new Vector2 (Screen.width, Screen.height);
					ApplyScreenPlacement ();
				} 
			} else {
				ApplyScreenPlacement ();
			}
		}
	}

	public void ApplyScreenPlacement ()
	{
		if (useResRatio) {
			transform.ScreenPlacement (anchor, new Vector2 (pixelOffset.x * DesignResolution.scaler.x, pixelOffset.y * DesignResolution.scaler.y), cam);	
		} else {
			transform.ScreenPlacement (anchor, pixelOffset, cam);	
		}
	}
}

public static class ScreenPlacementExtensions
{
	
	/// <summary>
	/// Search the specified target and name.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="name">Name.</param>
	public static Transform Search (this Transform target, string name)
	{
		if (target == null)
			return null;
		
		if (target.name == name)
			return target;
		
		for (int i = 0; i < target.childCount; ++i) {
			var result = Search (target.GetChild (i), name);
			
			if (result != null)
				return result;
		}
		
		return null;
	}

	/// <summary>
	/// Places a Transform at one of the 9 positions around the screen edge.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="position">
	/// A <see cref="TextAnchor"/>
	/// </param>
	public static void ScreenPlacement (this Transform target, TextAnchor aScreenPosition)
	{
		DoScreenPlacement (target.transform, aScreenPosition, Vector2.zero, Camera.main);
	}

	/// <summary>
	/// Places a Transform at one of the 9 positions around the screen edge.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="position">
	/// A <see cref="TextAnchor"/>
	/// </param>
	/// <param name="renderingCamera">
	/// A <see cref="Camera"/>
	/// </param>
	public static void ScreenPlacement (this Transform target, TextAnchor aScreenPosition, Camera renderingCamera)
	{
		DoScreenPlacement (target.transform, aScreenPosition, Vector2.zero, renderingCamera);
	}

	/// <summary>
	/// Places a Transform at one of the 9 positions around the screen edge.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="position">
	/// A <see cref="TextAnchor"/>
	/// </param>
	/// <param name="pixelsFromEdge">
	/// A <see cref="Vector2"/>
	/// </param>
	public static void ScreenPlacement (this Transform target, TextAnchor aScreenPosition, Vector2 pixelsFromEdge)
	{
		DoScreenPlacement (target.transform, aScreenPosition, pixelsFromEdge, Camera.main);
	}

	/// <summary>
	/// Places a Transform at one of the 9 positions around the screen edge.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="position">
	/// A <see cref="TextAnchor"/>
	/// </param>
	/// <param name="pixelsFromEdge">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <param name="renderingCamera">
	/// A <see cref="Camera"/>
	/// </param>
	public static void ScreenPlacement (this Transform target, TextAnchor aScreenPosition, Vector2 pixelsFromEdge, Camera renderingCamera)
	{
		DoScreenPlacement (target.transform, aScreenPosition, pixelsFromEdge, renderingCamera);
	}
	
	//Placement execution:
	private static void DoScreenPlacement (this Transform target, TextAnchor aScreenPosition, Vector2 pixelsFromEdge, Camera renderingCamera)
	{
		if (renderingCamera == null)
			renderingCamera = Camera.main;
		
		Vector3 screenPosition = Vector3.zero;
		float zPosition = -renderingCamera.transform.position.z + target.position.z;
		
		switch (aScreenPosition) {		
			
		//uppers:
		case TextAnchor.UpperLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (pixelsFromEdge.x, Screen.height - pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.UpperCenter:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 ((Screen.width / 2) + pixelsFromEdge.x, Screen.height - pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.UpperRight:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (Screen.width - pixelsFromEdge.x, Screen.height - pixelsFromEdge.y, zPosition));
			break;	
			
		//mids:
		case TextAnchor.MiddleLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (pixelsFromEdge.x, (Screen.height / 2) - pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.MiddleCenter:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 ((Screen.width / 2) + pixelsFromEdge.x, (Screen.height / 2) - pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.MiddleRight:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (Screen.width - pixelsFromEdge.x, (Screen.height / 2) - pixelsFromEdge.y, zPosition));
			break;
			
		//lowers:
		case TextAnchor.LowerLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (pixelsFromEdge.x, pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.LowerCenter:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 ((Screen.width / 2) + pixelsFromEdge.x, pixelsFromEdge.y, zPosition));
			break;
			
		case TextAnchor.LowerRight:
			screenPosition = renderingCamera.ScreenToWorldPoint (new Vector3 (Screen.width - pixelsFromEdge.x, pixelsFromEdge.y, zPosition));
			break;			
			
			
		}
		
		target.transform.position = screenPosition;
	}
}
