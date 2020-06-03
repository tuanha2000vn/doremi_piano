using UnityEngine;
using System.Collections;
using System;

public class KaraokeText : MonoBehaviour
{
	public MidiSeqKaraokeScript seq;

	void Start ()
	{
		seq.OnWord += (t) => {
			wordText += (t.newSentence ? "\n" : "") + (t.newVerse ? "\n\n" : "") + t.text;
		};
		seq.OnSentence += (t) => {
			sentenceText += (t.newVerse ? "\n" : "") + t.text + "\n";	
		};
		seq.OnVerse += (t) => {
			verseText += t.text + "\n\n";
		};
		seq.OnFinished += () => {
				
		};
	}

	string wordText = "";
	string sentenceText = "";
	string verseText = "";

	void OnGUI ()
	{
	
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Play", GUILayout.Width (80))) {
			seq.Play (false);	
		}
		
		if (GUILayout.Button ("Stop", GUILayout.Width (80))) {
			seq.Stop ();
			wordText = sentenceText = verseText = "";
		}

		TimeSpan t = TimeSpan.FromSeconds (seq.time);
		DateTime dateTime = new DateTime (t.Ticks);

		GUILayout.Label ("Time : " + dateTime.ToString ("mm:ss.f"));

		GUILayout.EndHorizontal ();
			
//		GUI.color = Color.grey;
		GUILayout.Label (wordText);
//		GUI.color = Color.blue;
//		GUILayout.Label (sentenceText);
//		GUI.color = Color.green;
//		GUILayout.Label (verseText);
	}
}
