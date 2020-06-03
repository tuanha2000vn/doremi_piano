using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent (typeof(Camera))]
[ExecuteInEditMode]
public class AspectRatio : MonoBehaviour
{

	public enum RatioEnum
	{
		Ratio_4_3,
		Ratio_16_9,
		Ratio_3_4,
		Ratio_9_16
	}

	Vector2 resolution = Vector2.zero;
	Vector2 aspectRatio = new Vector2 (4, 3);

	public RatioEnum ratio = RatioEnum.Ratio_4_3;
	public bool continuous = true;
	public bool onResolutionChange = true;
	public bool inEditor = true;

	public float originalCamSize = 5f;

	public bool drawGizmos = false;

	private Camera cam;
	// Use this for initialization
	void Awake ()
	{
		// obtain camera component so we can modify its viewport
		cam = GetComponent<Camera> ();
		resolution = new Vector2 (Screen.width, Screen.height);
		AdjustCameraViewPort ();
	}

	void Start ()
	{
		
	}

	void LateUpdate ()
	{
		if (Application.isEditor && !Application.isPlaying) {
			if (inEditor) {
				Apply ();
			}
		} else {
			Apply ();
		}
	}

	void Apply ()
	{
		if (continuous) {
			if (onResolutionChange) {
				if (Screen.width != resolution.x || Screen.height != resolution.y) {
					resolution = new Vector2 (Screen.width, Screen.height);
					AdjustCameraViewPort ();
				} 
			} else {
				AdjustCameraViewPort ();
			}
		}
	}

	void AdjustCameraViewPort ()
	{

		switch (ratio) {
		case RatioEnum.Ratio_4_3:
			aspectRatio = new Vector2 (4, 3);
			break;
		case RatioEnum.Ratio_16_9:
			aspectRatio = new Vector2 (16, 9);
			break;
		case RatioEnum.Ratio_3_4:
			aspectRatio = new Vector2 (3, 4);
			break;
		case RatioEnum.Ratio_9_16:
			aspectRatio = new Vector2 (9, 16);	
			break;
		}

		// set the desired aspect ratio 
		float targetaspect = aspectRatio.x / aspectRatio.y;

		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;
		
		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;

		float finalResult = 1f;

		switch (ratio) {
		case RatioEnum.Ratio_4_3:
			finalResult = originalCamSize * 1f / scaleheight;
			break;
		case RatioEnum.Ratio_16_9:
			finalResult = originalCamSize;
			break;
		case RatioEnum.Ratio_3_4:
			finalResult = originalCamSize;
			break;
		case RatioEnum.Ratio_9_16:
			finalResult = originalCamSize * 1f / scaleheight;	
			break;
		}

		if (finalResult != cam.orthographicSize) {
			cam.orthographicSize = finalResult;
		}
	}

	public virtual void OnDrawGizmos ()
	{
		if (!drawGizmos)
			return;
#if UNITY_EDITOR
		if (Camera.current == cam || Camera.current == SceneView.lastActiveSceneView.camera) {

		} else {
			return;
		}
#endif

		Matrix4x4 temp = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS (transform.position, transform.rotation, Vector3.one);

		float size = 1f;
		if (cam.orthographic) {
			float spread = cam.farClipPlane - cam.nearClipPlane;
			float center = (cam.farClipPlane + cam.nearClipPlane) * 0.5f;

			switch (ratio) {
			case RatioEnum.Ratio_4_3:
				size = originalCamSize * (4f / 3f) / (16f / 9f);
				Gizmos.DrawLine (
					new Vector3 (-cam.orthographicSize * cam.aspect, size, 1),
					new Vector3 (cam.orthographicSize * cam.aspect, size, 1) 
				);

				Gizmos.DrawLine (
					new Vector3 (-cam.orthographicSize * cam.aspect, -size, 1),
					new Vector3 (cam.orthographicSize * cam.aspect, -size, 1) 
				);

				Gizmos.DrawWireCube (new Vector3 (0, 0, center), new Vector3 (originalCamSize * 2f * (aspectRatio.x / aspectRatio.y), originalCamSize * 2f, spread));
				break;

			case RatioEnum.Ratio_3_4:
				size = originalCamSize * (9f / 16f);
				Gizmos.DrawLine (
					new Vector3 (-size, cam.orthographicSize, 1),
					new Vector3 (-size, -cam.orthographicSize, 1) 
				);
				
				Gizmos.DrawLine (
					new Vector3 (size, cam.orthographicSize, 1),
					new Vector3 (size, -cam.orthographicSize, 1) 
				);
				
				Gizmos.DrawWireCube (new Vector3 (0, 0, center), new Vector3 (originalCamSize * 2f * (3f / 4f), cam.orthographicSize * 2f, spread));
				break;
			
			case RatioEnum.Ratio_16_9:
				size = originalCamSize * (4f / 3f);
				Gizmos.DrawLine (
					new Vector3 (-size, cam.orthographicSize, 1),
					new Vector3 (-size, -cam.orthographicSize, 1) 
				);
				
				Gizmos.DrawLine (
					new Vector3 (size, cam.orthographicSize, 1),
					new Vector3 (size, -cam.orthographicSize, 1) 
				);
				
				Gizmos.DrawWireCube (new Vector3 (0, 0, center), new Vector3 (originalCamSize * 2f * (16f / 9f), cam.orthographicSize * 2f, spread));
				break;
			
			case RatioEnum.Ratio_9_16:
				size = originalCamSize * (9f / 16f) / (3f / 4f);
				Gizmos.DrawLine (
					new Vector3 (-cam.orthographicSize * cam.aspect, size, 1),
					new Vector3 (cam.orthographicSize * cam.aspect, size, 1) 
				);
				
				Gizmos.DrawLine (
					new Vector3 (-cam.orthographicSize * cam.aspect, -size, 1),
					new Vector3 (cam.orthographicSize * cam.aspect, -size, 1)
				);
				
				Gizmos.DrawWireCube (new Vector3 (0, 0, center), new Vector3 (originalCamSize * 2f * (aspectRatio.x / aspectRatio.y), originalCamSize * 2f, spread));
				break;
			}
		} 
		Gizmos.matrix = temp;
	}
}
