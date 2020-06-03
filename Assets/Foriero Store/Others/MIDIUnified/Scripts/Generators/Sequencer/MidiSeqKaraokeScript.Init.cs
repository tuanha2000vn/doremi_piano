using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForieroEngine.MIDIUnified;
using ForieroEngine.MIDIUnified.Midi;
using System.Text.RegularExpressions;

public partial class MidiSeqKaraokeScript : MonoBehaviour, IMidiEvents
{
	private bool Initialize ()
	{
		tracks = new List<IList<MidiEvent>> ();
		
		eventPos = new int[0];
		endOfTrack = new bool[0];
		muteTrack = new bool[0];
		
		words = new List<WordText> ();
		wordPos = 0;
		wordOffsetPos = 0;
		sentences = new List<SentenceText> ();
		sentencePos = 0;
		verses = new List<MidiText> ();
		versePos = 0;
		midiFile = null;
		bar = 0;
		barCount = 0;
		bars = new List<Bar> ();
		state = State.None;
		fractionalTicks = 0;
		deltaTimeNumerator = 0f;
		deltaTimeRest = 0f;
		time = 0f;
		ticks = 0f;
		totalTime = 0f;
		lastDeltaTime = 0f;
		lastDeltaTicks = 0f;

		if (midiFileBytes == null)
			return false;
			
		if (midiFileBytes.Length > 0) {
			midiFile = new MidiFile (new MemoryStream (midiFileBytes), false);
						
			for (int i = 0; i < midiFile.Tracks; i++) {
				tracks.Add (midiFile.Events.GetTrackEvents (i));
//				//END OF TRACK FIX//
//				if(i != 0) continue;
//				for(int k = tracks[i].Count - 1; k >=0; k--){
//					MidiEvent me = tracks[i][k];
//					if(me.CommandCode == MidiCommandCode.MetaEvent){
//						MetaEvent mte = me as MetaEvent;
//						if(mte.MetaEventType == MetaEventType.EndTrack) {
//							Debug.Log("ENDOFOFOFO");
//						} 
//					}
//				}
			}
									
			eventPos = new int[midiFile.Tracks];
			endOfTrack = new bool[midiFile.Tracks];
			muteTrack = new bool[midiFile.Tracks];
			PPQN = midiFile.DeltaTicksPerQuarterNote;

#region Words, Sentences, Verses			
			var events = from e in tracks [lyricTrack]
			             where (e is TextEvent) && e.AbsoluteTime > 0
			             select e;
			
//			bool sentence = true;
//			bool verse = true;
			bool lastWordHadSentenceSign = false;
			//string test = "";
			foreach (TextEvent e in events) {

				string cmdNewVerse = "[NV]";
				string cmdNewSentence = "[NS]";
				string cmdNewLine = "[NL]";
//				string cmdPhonemNone = "[P]";
//				string cmdPhonemA = "[PA]";
//				string cmdPhonemE = "[PE]";
//				string cmdPhonemI = "[PI]";
//				string cmdPhonemO = "[PO]";
//				string cmdPhonemU = "[PU]";
//				string cmdBridgeBegin = "[BB]";
//				string cmdBridgeEnd = "[BE]";

				//test += e.Text;
				//Debug.Log (test);

				// tohle je standardne pouzivano v karaoke midi filech, tak pro compatibilitu s nima to prevadim do naseho systemu //
				string t = e.Text.Replace ("\\", cmdNewVerse).Replace ("/", cmdNewSentence);

				if (forceSentenceNewLine && !t.Contains ("[NL]")) {
					t = t.Replace (".", ".[NL]").Replace ("!", "![NL]").Replace ("?", "?[NL]");
				}

				if (forceCommaNewLine && !t.Contains ("NL")) {
					t = t.Replace (",", ".[NL]");
				}

				Regex regexCommands = new Regex ("\\[(.*?)\\]");

				MatchCollection matches = regexCommands.Matches (t);

				List<string> commands = new List<string> ();

				foreach (Match m in matches) {
					commands.Add (m.Value);
				}

				words.Add (new WordText () { 
					text = regexCommands.Replace (t, ""), 
					absoluteStartTime = e.AbsoluteTime, 
					deltaTime = e.DeltaTime,
					newSentence = commands.Contains (cmdNewSentence),
					newVerse = commands.Contains (cmdNewVerse),
					commands = commands
				});
							
				if (commands.Contains (cmdNewVerse) || commands.Contains (cmdNewSentence) || (forceSentences && lastWordHadSentenceSign)) {
					lastWordHadSentenceSign = false;
					sentences.Add (new SentenceText () {
						text = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), ""),
						absoluteStartTime = e.AbsoluteTime,
						newVerse = commands.Contains (cmdNewVerse),
						commands = commands
					});
				} else {
					if (sentences.Count > 0) {
						string sentenceHelper = "";
						if (sentences.Last ().text.EndsWith ("\n")) {
							sentenceHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "").TrimStart (' ');
						} else if (sentences.Last ().text.EndsWith ("\n" + " ")) {
							sentences.Last ().text = sentences.Last ().text.TrimEnd (' ');
							sentenceHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "");
						} else {
							sentenceHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "");
						}
						sentences.Last ().text += sentenceHelper;
					}
				}
								
				if (commands.Contains (cmdNewVerse)) {	
					verses.Add (new MidiText () { 
						text = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), ""),
						absoluteStartTime = e.AbsoluteTime,
						commands = commands
					});
				} else {
					if (verses.Count > 0) {
						string verseHelper = "";
						if (verses.Last ().text.EndsWith ("\n")) {
							verseHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "").TrimStart (' ');
						} else if (verses.Last ().text.EndsWith ("\n" + " ")) {
							verses.Last ().text = verses.Last ().text.TrimEnd (' ');
							verseHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "");
						} else {
							verseHelper = regexCommands.Replace (t.Replace (cmdNewLine, "\n"), "");
						}

						verses.Last ().text += verseHelper;
					}
				}

				lastWordHadSentenceSign = t.Contains (".") || t.Contains ("!") || t.Contains ("?");
			}
#endregion				
			
			while (state != State.Finished) {
				UpdateBars ();	
			}
				
			barCount = bars.Count;
			time = 0;
			ticks = 0;
			bar = 0;
			fractionalTicks = 0;
			deltaTimeNumerator = 0f;
			deltaTimeRest = 0f;
			lastDeltaTime = 0f;
			lastDeltaTicks = 0f;
			
			eventPos = new int[midiFile.Tracks];
			endOfTrack = new bool[midiFile.Tracks];
			muteTrack = new bool[midiFile.Tracks];
									
			state = State.None;
		
			return true;
		}
		return false;
	}

	public void Initialize (TextAsset aMidiFile, AudioClip aVocalClip, AudioClip aMusicClip)
	{
		midiFileTextAsset = aMidiFile;
		Initialize (aMidiFile == null ? null : aMidiFile.bytes, aVocalClip, aMusicClip);
	}

	public void Initialize (byte[] aMidiFileBytes, AudioClip aVocalClip, AudioClip aMusicClip)
	{
		initialized = false;
		Stop ();

		if (audioVocals) {
			audioVocals.clip = vocalsClip = aVocalClip;
		}

		if (audioVocals) {
			audioVocals.volume = vocalsVolume;
		}

		if (audioMusic) {
			audioMusic.clip = musicClip = aMusicClip;
		}

		if (audioMusic) {
			audioMusic.volume = musicVolume;
		}
		midiFileBytes = aMidiFileBytes;
		initialized = Initialize ();

		ResetSequencer ();

		SetSpeed (speed);

		if (initialized) {
			if (OnMidiLoaded != null) {
				OnMidiLoaded ();
			}
		}

		if (OnInitialized != null) {
			OnInitialized ();
		}
	}
}
