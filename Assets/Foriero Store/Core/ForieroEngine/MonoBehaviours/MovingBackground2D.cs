using UnityEngine;
using System.Collections.Generic;
using ForieroEngine;
using ForieroEngine.Extensions;

public class MovingBackground2D : MonoBehaviour
{

	class EdgeTransforms
	{
		public SpriteRenderer spriteRenderer;
		public Transform left;
		public Transform right;
	}

	List<EdgeTransforms> edges = new List<EdgeTransforms> ();

	public enum DirectionEnum
	{
		None,
		Right,
		Left
	}

	public enum TypeEnum
	{
		Seamless,
		Objects
	}

	public Camera cam;
	public TypeEnum type = TypeEnum.Seamless;
	public float speed = 0.1f;
	public float timeScale = 1f;
	public bool reverse = false;

	[HideInInspector]
	public DirectionEnum direction = DirectionEnum.None;

	[HideInInspector]
	public float x = 0f;
	float xUpdate = 0f;

	[Tooltip ("Objects from left to right")]
	public SpriteRenderer[] objects;

	public Transform leftEdge;
	public Transform rightEdge;

	Vector3 leftScreenPoint = Vector3.zero;
	Vector3 rightScreenPoint = Vector3.zero;

	SpriteRenderer spriteRenderer;
	EdgeTransforms edgeTransfomrs;

	void Start ()
	{
		if (!cam)
			cam = Camera.main;

		if (!leftEdge || !rightEdge)
			return;

		switch (type) {
		case TypeEnum.Objects:
			leftScreenPoint = leftEdge.position;
			rightScreenPoint = rightEdge.position;
			break;
		case TypeEnum.Seamless:
			leftScreenPoint = cam.ViewportToWorldPoint (new Vector3 (0, 0.5f, 0));
			rightScreenPoint = cam.ViewportToWorldPoint (new Vector3 (1, 0.5f, 0));
			SetUpSeamlessTransfomrs ();	
			break;
		}
	}

	void SetUpSeamlessTransfomrs ()
	{
		spriteRenderer = objects [0];

		edgeTransfomrs = new EdgeTransforms ();
		edgeTransfomrs.spriteRenderer = spriteRenderer;
		edgeTransfomrs.left = leftEdge;
		leftEdge.parent = spriteRenderer.transform;
		edgeTransfomrs.right = CreateEdgeTransformPoint (spriteRenderer.transform,
			objects [1].bounds.center.x - objects [1].bounds.extents.x / 2f,
			spriteRenderer.bounds.center.x + spriteRenderer.bounds.extents.x / 2f,
			spriteRenderer.transform.position.y,
			spriteRenderer.transform.position.z);
		edges.Add (edgeTransfomrs);	

		for (int i = 1; i < objects.Length - 1; i++) {
			spriteRenderer = objects [i];
			edgeTransfomrs = new EdgeTransforms ();
			edgeTransfomrs.spriteRenderer = spriteRenderer;
			edgeTransfomrs.left = CreateEdgeTransformPoint (spriteRenderer.transform,
				spriteRenderer.bounds.center.x - spriteRenderer.bounds.extents.x / 2f,
				objects [i - 1].bounds.center.x + objects [i - 1].bounds.extents.x / 2f,
				spriteRenderer.transform.position.y,
				spriteRenderer.transform.position.z);
			edgeTransfomrs.right = CreateEdgeTransformPoint (spriteRenderer.transform,
				objects [i + 1].bounds.center.x - objects [i + 1].bounds.extents.x / 2f,
				spriteRenderer.bounds.center.x + spriteRenderer.bounds.extents.x / 2f,
				spriteRenderer.transform.position.y, 
				spriteRenderer.transform.position.z);
			edges.Add (edgeTransfomrs);
		}

		spriteRenderer = objects [objects.Length - 1];
		edgeTransfomrs = new EdgeTransforms ();
		edgeTransfomrs.spriteRenderer = spriteRenderer;
		edgeTransfomrs.left = CreateEdgeTransformPoint (spriteRenderer.transform,
			spriteRenderer.bounds.center.x - spriteRenderer.bounds.extents.x / 2f,
			objects [objects.Length - 2].bounds.center.x + objects [objects.Length - 2].bounds.extents.x / 2f,
			spriteRenderer.transform.position.y,
			spriteRenderer.transform.position.z);
		edgeTransfomrs.right = rightEdge;
		rightEdge.parent = spriteRenderer.transform;
		edges.Add (edgeTransfomrs);
	}

	Transform CreateEdgeTransformPoint (Transform parent, float left, float right, float y, float z)
	{
		GameObject go = new GameObject ();
		go.transform.parent = parent;
		go.transform.position = new Vector3 ((left + right) / 2f, y, z);
		return go.transform;
	}

	// Update is called once per frame
	void Update ()
	{
		x += speed * timeScale * (reverse ? -1f : 1f) * Time.deltaTime;

		if (x == xUpdate) {
			direction = DirectionEnum.None;
		} else if (x < xUpdate)
			direction = DirectionEnum.Left;
		else
			direction = DirectionEnum.Right;

		switch (type) {
		case TypeEnum.Seamless:
			UpdateSeemlessTransforms ();
			break;
		case TypeEnum.Objects:
			UpdateObjectTransforms ();
			break;
		}
		xUpdate = x;
	}

	void UpdateObjectTransforms ()
	{
		for (int i = 0; i < objects.Length; i++) {
			objects [i].transform.Translate (Vector3.right * speed * timeScale * (reverse ? -1f : 1f) * Time.deltaTime, Space.World);
		}

		if (!leftEdge || !rightEdge)
			return;

		for (int i = 0; i < objects.Length; i++) {
			spriteRenderer = objects [i];
			switch (direction) {
			case DirectionEnum.Right:
				if (spriteRenderer.transform.position.x > rightScreenPoint.x) {
					spriteRenderer.transform.position = new Vector3 (leftScreenPoint.x, spriteRenderer.transform.position.y, spriteRenderer.transform.position.z);	
				}
				break;
			case DirectionEnum.Left:
				if (spriteRenderer.transform.position.x < leftScreenPoint.x) {
					spriteRenderer.transform.position = new Vector3 (rightScreenPoint.x, spriteRenderer.transform.position.y, spriteRenderer.transform.position.z);
				}
				break;
			}
		}
	}

	void UpdateSeemlessTransforms ()
	{
		for (int i = 0; i < edges.Count; i++) {
			edgeTransfomrs = edges [i];
			edgeTransfomrs.spriteRenderer.transform.Translate (Vector3.right * speed * timeScale * (reverse ? -1f : 1f) * Time.deltaTime, Space.World);
		}

		for (int i = 0; i < edges.Count; i++) {
			edgeTransfomrs = edges [i];

			switch (direction) {
			case DirectionEnum.Right:
				if (edgeTransfomrs.left.position.x > rightScreenPoint.x) {
					var x = GetNextEdge (i).left.position.x - edgeTransfomrs.spriteRenderer.transform.position.x.Distance (edgeTransfomrs.right.position.x);
					edgeTransfomrs.spriteRenderer.transform.position = new Vector3 (x, edgeTransfomrs.spriteRenderer.transform.position.y, edgeTransfomrs.spriteRenderer.transform.position.z);	
				}
				break;
			case DirectionEnum.Left:
				if (edgeTransfomrs.right.position.x < leftScreenPoint.x) {
					var x = GetPreviousEdge (i).right.position.x + edgeTransfomrs.spriteRenderer.transform.position.x.Distance (edgeTransfomrs.left.position.x);
					edgeTransfomrs.spriteRenderer.transform.position = new Vector3 (x, edgeTransfomrs.spriteRenderer.transform.position.y, edgeTransfomrs.spriteRenderer.transform.position.z);
				}
				break;
			}
		}
	}

	EdgeTransforms GetPreviousEdge (int index)
	{
		if (index - 1 < 0) {
			return edges [edges.Count - 1];
		} else {
			return edges [index - 1];
		}
	}

	EdgeTransforms GetNextEdge (int index)
	{
		if (index + 1 > edges.Count - 1) {
			return edges [0];
		} else {
			return edges [index + 1];
		}
	}
}
