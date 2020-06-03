using UnityEngine;
using System.Collections;
using System.IO;
using System;


namespace ForieroEditor.Extensions
{
	public static partial class ForieroEditorExtensions
	{
		public static Stream GetMemoryStream (this string s)
		{
			MemoryStream stream = new MemoryStream ();
			StreamWriter writer = new StreamWriter (stream);
			writer.Write (s);
			writer.Flush ();
			stream.Position = 0;
			return stream;
		}

		public static bool Contains (this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf (toCheck, comp) >= 0;
		}
	}
}
