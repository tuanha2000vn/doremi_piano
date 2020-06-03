using UnityEngine;
using System.Collections;

using ForieroEngine.MIDIUnified;

public class ToneCube2D : MonoBehaviour
{
	public int midiIndex = 0;
	public GameObject PREFAB_Particles;
	public GameObject PREFAB_Text;
	float yForceMultp = 2f;

	Rigidbody2D rb;

	GameObject particlesObject;
	ParticleSystem particlesObjectPS;

	GameObject textObject;
	TextMesh textObjectTM;

	void OnMouseDown ()
	{
		MidiOut.NoteOn (midiIndex, 100, 0);
	}

	void OnMouseUp ()
	{
		MidiOut.NoteOff (midiIndex, 0);
	}

	ParticleSystem.MainModule mainModule;

	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();

		MidiOut.ShortMessageEvent += (aCommnad, aData1, aData2) => {
			if (aCommnad.ToMidiCommand () == 144 && aData1 == midiIndex) {
				rb.AddRelativeForce (new Vector2 (0f, aData2 * yForceMultp));
				if (!particlesObject) {
					particlesObject = Instantiate (PREFAB_Particles, transform.position, Quaternion.identity) as GameObject;
					particlesObject.transform.SetParent (transform);
					particlesObjectPS = particlesObject.GetComponent<ParticleSystem> ();
					mainModule = particlesObjectPS.main;
					mainModule.startColor = aData1.ToMidiColor ();
				} else {
					var emission = particlesObjectPS.emission;
					emission.enabled = true;
				}

				if (!textObject) {
					textObject = Instantiate (PREFAB_Text, new Vector3 (transform.position.x, 0, 0), Quaternion.identity) as GameObject;
					textObjectTM = textObject.GetComponent<TextMesh> ();
					textObjectTM.text = MidiConversion.GetToneNameFromMidiIndex (midiIndex);
					textObjectTM.color = aData1.ToMidiColor ();
				} else {
					textObject.SetActive (true);
				}

				//Debug.Log (MidiConversion.GetToneNameFromMidiIndex (midiIndex));
			} 

			if ((aCommnad.ToMidiCommand () == 128 && aData1 == midiIndex) || (aCommnad.ToMidiCommand () == 144 && aData1 == midiIndex && aData2 == 0)) {
				if (particlesObjectPS) {
					var emission = particlesObjectPS.emission;
					emission.enabled = false;
				}

				if (textObject) {
					textObject.SetActive (false);
				}
			}
		};
	}
}
