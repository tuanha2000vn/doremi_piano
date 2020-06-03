using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public class ToneCubes2D : MonoBehaviour
{

	public GameObject PREFAB_ToneCube2D;
	public Camera cam;
	public Transform ground;

	public int midiStart = 20;
	public int cubeCount = 80;

	void Start ()
	{
		float size = cam.orthographicSize * cam.aspect * 2f / cubeCount;

		float start = -cam.orthographicSize * cam.aspect + size / 2f;

		for (int i = 0; i < cubeCount; i++) {
			GameObject go = Instantiate (PREFAB_ToneCube2D, new Vector3 (start + i * size, -cam.orthographicSize + size / 2f, 0), Quaternion.identity) as GameObject;
			go.transform.localScale = new Vector3 (size * 100, size * 100, size * 100);

			go.transform.SetParent (gameObject.transform);

			go.GetComponent<SpriteRenderer> ().color = (midiStart + i).ToMidiColor ();

			go.AddComponent<BoxCollider2D> ();

			ToneCube2D toneCube2D = go.GetComponent<ToneCube2D> ();

			toneCube2D.midiIndex = midiStart + i;
		}
	}
}
