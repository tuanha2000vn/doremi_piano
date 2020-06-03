using UnityEngine;
using System.Collections;
using ForieroEngine.MIDIUnified;

public class FlyingObjects : MonoBehaviour
{
    public GameObject prefab;
    public bool createCollider = false;
    public int count = 0;
    public int startMidiIndex = 21;
    public float size = 1.0f;
    public float startHeight = 1.1f;
    public float heightConstraint = 10f;
    public float distance = 1.1f;
    public float forceMultiplicator = 1f;
    public GameObject engine;

    private FlyingObject[] flyingObjects = new FlyingObject[0];
    private BoxCollider boxCollider;

    MidiEvents midiEvents;

    void Awake()
    {
        if (createCollider)
        {
            if (!GetComponent<Collider>())
                boxCollider = (BoxCollider)gameObject.AddComponent<BoxCollider>();
            else
                boxCollider = (BoxCollider)GetComponent<Collider>();
            boxCollider.size = new Vector3(count * distance, 1f, 1f);
            boxCollider.center = new Vector3((-1f) * (((count * distance) / 2) - (distance / 2)), 0f, 0f);
        }
        if (prefab)
        {
            flyingObjects = new FlyingObject[count];
            for (int i = 0; i < count; i++)
            {
                flyingObjects[i] = (Instantiate(prefab) as GameObject).GetComponent<FlyingObject>();
                flyingObjects[i].midiIndex = startMidiIndex + i;
                flyingObjects[i].volume = 80; //VOLUME IS SET AS BYTE 0-127
                flyingObjects[i].NoteOn += NoteOnHandler;
                flyingObjects[i].transform.parent = transform;
                flyingObjects[i].transform.position = new Vector3((transform.position.x + (i * distance)), transform.position.y + startHeight, transform.position.z);
                flyingObjects[i].transform.localScale *= size;
                flyingObjects[i].GetComponent<Renderer>().material.color = (startMidiIndex + i).ToMidiColor();
                flyingObjects[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", (startMidiIndex + i).ToMidiColor());
            }
        }
    }

    void Start()
    {
        midiEvents = new MidiEvents();
        midiEvents.AddGenerator(MidiInput.singleton as IMidiEvents);
        midiEvents.AddGenerator(MidiKeyboardInput.singleton as IMidiEvents);
        midiEvents.AddGenerator(MidiPlayMakerInput.singleton as IMidiEvents);
        midiEvents.AddGenerator(MidiSeqKaraokeScript.singleton as IMidiEvents);
        midiEvents.NoteOnEvent += NoteOnHandler;
    }

    void NoteOnHandler(int aMidiIndex, int aVolume, int channel)
    {
        if (aMidiIndex - startMidiIndex >= 0 && aMidiIndex - startMidiIndex < flyingObjects.Length)
        {
            if (flyingObjects[aMidiIndex - startMidiIndex].rigidBody)
            {
                if (flyingObjects[aMidiIndex - startMidiIndex].transform.position.y < heightConstraint)
                {
                    flyingObjects[aMidiIndex - startMidiIndex].rigidBody.AddForce(new Vector3(0f, (float)aVolume * forceMultiplicator, 0f));
                }
            }
            if (engine)
                Instantiate(engine, flyingObjects[aMidiIndex - startMidiIndex].transform.position, Quaternion.identity);
        }
    }

}
