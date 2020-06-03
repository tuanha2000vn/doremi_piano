using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class CSharpSynth : MonoBehaviour
{
    public class MFile : AudioSynthesis.IResource
    {
        private byte[] file;
        private string fileName;

        public MFile(byte[] file, string fileName)
        {
            this.file = file;
            this.fileName = fileName;
        }

        public string GetName()
        {
            return fileName;
        }

        public bool DeleteAllowed()
        {
            return false;
        }

        public bool ReadAllowed()
        {
            return true;
        }

        public bool WriteAllowed()
        {
            return false;
        }

        public void DeleteResource()
        {
            return;
        }

        public Stream OpenResourceForRead()
        {
            return new MemoryStream(file);
        }

        public Stream OpenResourceForWrite()
        {
            return null;
        }
    }
}
