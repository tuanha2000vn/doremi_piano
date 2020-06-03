using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static Rect Add (this Rect aRectA, Rect aRectB)
		{
			return new Rect (aRectA.x + aRectB.x, aRectA.y + aRectB.y, aRectA.width + aRectB.width, aRectA.height + aRectB.height);
		}
	}
}
