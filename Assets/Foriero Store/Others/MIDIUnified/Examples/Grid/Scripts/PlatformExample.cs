using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.MIDIUnified;

public class PlatformExample : MonoBehaviour
{

	public GameObject musicCubePrefab;
	public float distance = 1f;
	
	public Material[] materials = new Material[12];
	
	MusicCube[] musicCubes = new MusicCube[8 * 12];

	void Awake ()
	{
		for (int i = 0; i < 8; i++) {
			for (int k = 0; k < 12; k++) {
				GameObject go = (GameObject)Instantiate (musicCubePrefab);
				go.transform.position = new Vector3 (k * distance - 4 * distance, 0f, i * distance - 4 * distance);
				go.GetComponent<Renderer> ().material = materials [k];
				go.GetComponent<Renderer> ().material.color = (i * 12 + k).ToMidiColor ();
				go.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", (i * 12 + k).ToMidiColor ());
				go.transform.parent = transform;
				musicCubes [i * 12 + k] = go.GetComponent<MusicCube> () as MusicCube;
				musicCubes [i * 12 + k].midiIndex = 8 * 12 - (4 + i * 12 + (11 - k));
			}
		}
	}

	List<MusicCube> cubes = new List<MusicCube> ();

	void OnTap (int finger, Vector2 pos)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			GameObject go = hit.transform.gameObject;
			MusicCube mc = go.GetComponent<MusicCube> ();
			if (mc != null) {
				mc.Play ();
				mc.finger = finger;
				cubes.Add (mc);
			}
		}
	}

	void OnUp (int finger, Vector2 pos, float timeHeld)
	{
		MusicCube toberemoved = null;
		foreach (MusicCube mc in cubes) {
			if (mc.finger == finger) {
				toberemoved = mc;
				break;
			}
		}
		if (toberemoved != null) {
			toberemoved.Stop ();
			cubes.Remove (toberemoved);	
		}
	}

	Touch touch;
	Ray ray;
	#pragma warning disable 414
	RaycastHit hit;
	#pragma warning restore 414

	bool[] touchPhaseBegan = new bool[20];

	void Update ()
	{
		for (int i = 0; i < Input.touches.Length; i++) {
			touch = Input.touches [i];
			ray = Camera.main.ScreenPointToRay (touch.position);
			hit = new RaycastHit ();
			if (touch.phase == TouchPhase.Began && !touchPhaseBegan [touch.fingerId]) {
				if (Physics.Raycast (ray, out hit, 100)) {
					touchPhaseBegan [touch.fingerId] = true;
					OnTap (touch.fingerId, touch.position);
				} 
		
			} else if (touch.phase == TouchPhase.Moved) {
				
			} else if (touch.phase == TouchPhase.Ended) {
				OnUp (touch.fingerId, touch.position, 0f);
			}
			
		}
	}
}
