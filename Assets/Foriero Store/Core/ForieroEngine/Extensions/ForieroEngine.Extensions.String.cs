using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static string RemoveDiacritics (this string stIn)
		{
			string stFormD = stIn.Normalize (NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder ();

			for (int ich = 0; ich < stFormD.Length; ich++) {
				switch (CharUnicodeInfo.GetUnicodeCategory (stFormD [ich])) {
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
				case UnicodeCategory.EnclosingMark:
					break;
				default : 
					sb.Append (stFormD [ich]);
					break;
				}
			}

			return(sb.ToString ().Normalize (NormalizationForm.FormC));	
		}

		public static char GetAccent (this string stIn)
		{
			string stFormD = stIn.Normalize (NormalizationForm.FormD);

			StringBuilder sb = new StringBuilder ();

			for (int ich = 0; ich < stFormD.Length; ich++) {
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory (stFormD [ich]);
				if (uc == UnicodeCategory.NonSpacingMark) {
					sb.Append (stFormD [ich]);
				}
			}

			if (sb.Length > 0) {
				return sb.ToString ().Normalize (NormalizationForm.FormC) [0];
			} else {
				return default(char);	
			}
		}

		public static bool IsDiacriticsed (this string stIn)
		{
			string stFormD = stIn.Normalize (NormalizationForm.FormD);

			for (int ich = 0; ich < stFormD.Length; ich++) {
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory (stFormD [ich]);
				if (uc == UnicodeCategory.NonSpacingMark) {
					return true;		
				}
			}

			return false;	
		}

		public static string FixNewLine (this string s)
		{
			return s.Replace ("\r", "\n").Replace (((char)3).ToString (), "\n");
		}

		/// <summary>
		/// Remove HTML from string with Regex.
		/// </summary>
		public static string StripTagsRegex (this string source)
		{
			return Regex.Replace (source, "<.*?>", string.Empty);
		}

		/// <summary>
		/// Compiled regular expression for performance.
		/// </summary>
		//static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
		
		/// <summary>
		/// Remove HTML from string with compiled Regex.
		/// </summary>
		//public static string StripTagsRegexCompiled(this string source)
		//{
		//	return _htmlRegex.Replace(source, string.Empty);
		//}
		
		/// <summary>
		/// Remove HTML tags from string using char array.
		/// </summary>
		public static string StripTagsCharArray (this string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;
			
			for (int i = 0; i < source.Length; i++) {
				char let = source [i];
				if (let == '<') {
					inside = true;
					continue;
				}
				if (let == '>') {
					inside = false;
					continue;
				}
				if (!inside) {
					array [arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string (array, 0, arrayIndex);
		}

		public static string[] Split (this string s, string separator)
		{
			return s.Split (new string[] { separator }, System.StringSplitOptions.None);
		}

		public static int OccurenceCount (this string str, string val)
		{  
			int occurrences = 0;
			int startingIndex = 0;

			while ((startingIndex = str.IndexOf (val, startingIndex)) >= 0) {
				++occurrences;
				++startingIndex;
			}

			return occurrences;
		}

		public static int NthIndexOf (this string target, string value, int n)
		{
            
			string[] result = target.Split (value);
			n--;
			if (n >= 0 && n < result.Length) {
				int index = 0;
				for (int i = 0; i <= n; i++) {
					index += result [i].Length + value.Length; 
				}
				return index - value.Length;
			} else {
				return -1;
			}
		}

		public static bool Contains (this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf (toCheck, comp) >= 0;
		}
	}
}
