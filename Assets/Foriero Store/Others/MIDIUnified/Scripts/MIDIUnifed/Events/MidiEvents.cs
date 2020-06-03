using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.MIDIUnified
{

    public partial class MidiEvents
    {

        public string name = "";
        public int id = 0;


        List<IMidiEvents> generators = new List<IMidiEvents>();

        void ShortMessage(int aCommand, int aData1, int aData2)
        {
            AddShortMessage(aCommand, aData1, aData2);
        }

        public void AddGenerator(IMidiEvents aGenerator)
        {
            if (aGenerator != null)
            {
                if (!generators.Contains(aGenerator))
                {
                    generators.Add(aGenerator);
                    aGenerator.ShortMessageEvent += ShortMessage;
                    if (MIDISettings.instance.debug)
                    {
                        Debug.Log("Generator ADDED");
                    }
                }
                else
                {
                    if (MIDISettings.instance.debug)
                    {
                        Debug.LogWarning("MidiEvents already contains this generate!");
                    }
                }
            }
            else
            {
                if (MIDISettings.instance.debug)
                {
                    Debug.LogError("IMidieEvents Generator is NULL!");
                }
            }
        }

        public void RemoveGenerator(IMidiEvents aGenerator)
        {
            if (generators.Contains(aGenerator))
            {
                aGenerator.ShortMessageEvent -= ShortMessage;
                generators.Remove(aGenerator);
                //Debug.Log("Generator REMOVED");
            }
        }

        public MidiEvents()
        {

        }

        ~MidiEvents()
        {
            foreach (IMidiEvents generator in generators)
            {
                if (generator != null) generator.ShortMessageEvent -= ShortMessage;
            }
        }

        public void Dispose()
        {
            foreach (IMidiEvents generator in generators)
            {
                if (generator != null) generator.ShortMessageEvent -= ShortMessage;
            }
        }
    }
}
