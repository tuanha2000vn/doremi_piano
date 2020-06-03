using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.Extensions;

[CustomEditor (typeof(MovingBackground2D))]
public class MovingBackground2DEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		if (GUILayout.Button ("Set Up")) {
			MovingBackground2D mb2D = target as MovingBackground2D;
									
			List<SpriteRenderer> renderers = new List<SpriteRenderer> ();

			foreach (Transform t in mb2D.transform) {
				SpriteRenderer renderer = t.gameObject.GetComponent<SpriteRenderer> ();
				if (renderer)
					renderers.Add (renderer);
			}

			renderers.Sort ((a, b) => {
				return a.transform.position.x.CompareTo (b.transform.position.x);
			});

			mb2D.objects = renderers.ToArray ();

			Transform left = mb2D.transform.Find ("LEFT");
			Transform right = mb2D.transform.Find ("RIGHT");

			if (!left) {
				left = new GameObject ("LEFT").transform;
				left.SetParent (mb2D.transform);
			}

			mb2D.leftEdge = left;

			if (!right) {
				right = new GameObject ("RIGHT").transform;
				right.SetParent (mb2D.transform);
			}

			mb2D.rightEdge = right;

			Rect leftRect = mb2D.cam.ToWorldRect (renderers.First ());

			left.position = new Vector3 (leftRect.x, renderers.First ().transform.position.y, renderers.First ().transform.position.z);

			Rect rightRect = mb2D.cam.ToWorldRect (renderers.Last ());

			right.position = new Vector3 (rightRect.x + rightRect.width, renderers.Last ().transform.position.y, renderers.Last ().transform.position.z);
		}
	}
}
