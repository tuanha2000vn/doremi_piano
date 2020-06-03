using System.Collections.Generic;

namespace MusicXml.Domain
{
	public class Measure
	{
		internal Measure()
		{
		    Number = -1;
		    StartPos = 0;
		    Duration = 0;
			MeasureElements = new List<MeasureElement>();
		}

	    public int Number { get; internal set; }
        public float StartPos { get; internal set; }
        public float Duration { get; internal set; }
        // This can be any musicXML element in the measure tag, i.e. note, backup, etc
        public List<MeasureElement> MeasureElements { get; internal set; }
		
		public MeasureAttributes Attributes { get; internal set; }
	}
}
