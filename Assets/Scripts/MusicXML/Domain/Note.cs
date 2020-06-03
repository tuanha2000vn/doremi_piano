namespace MusicXml.Domain
{
	public class Note
	{
		internal Note()
		{
			Type = string.Empty;
			Duration = 0;
		    StartPos = 0;
		    Finger = 0;
		    Pmn = 60;
			Voice = -1;
			Staff = -1;
		    IsArpeggiate = false;
			IsChordTone = false;
		    IsGrace = false;
		    Tied = string.Empty;
		}

        public string Type { get; internal set; }
		
		public int Voice { get; internal set; }

		public int Duration { get; internal set; }

		public float StartPos { get; internal set; }

		public int Finger { get; internal set; }

		public int Pmn { get; internal set; }

		public Lyric Lyric { get; internal set; }
		
		public Pitch Pitch { get; internal set; }

		public int Staff { get; internal set; }

		public bool IsArpeggiate { get; internal set; }

		public bool IsChordTone { get; internal set; }

		public bool IsGrace { get; internal set; }

		public bool IsRest { get; internal set; }
		
        public string Accidental { get; internal set; }

	    public string Tied { get; internal set; }

    }
}
