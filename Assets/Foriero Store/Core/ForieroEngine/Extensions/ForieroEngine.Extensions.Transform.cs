using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
		public static Transform Search (this Transform target, string name)
		{
			if (target == null)
				return null;

			if (target.name == name)
				return target;

			for (int i = 0; i < target.childCount; ++i) {
				var result = Search (target.GetChild (i), name);

				if (result != null)
					return result;
			}

			return null;
		}

		public static void CenterPivot (this Transform transform, bool recursive, bool twoD)
		{
			Vector3 sum = new Vector3 (0, 0, 0);
			int count = 0;

			foreach (Transform t in transform) {
				if (recursive) {
					t.CenterPivot (recursive, twoD);
				}

				sum = new Vector3 (sum.x + t.position.x, sum.y + t.position.y, twoD ? 0 : sum.z + t.position.z);
				count++;
			}

			if (count == 0) {
				return;
			}

			Vector3 center = new Vector3 (sum.x / (float)count, sum.y / (float)count, twoD ? transform.position.z : sum.z / (float)count);
			Vector3 diff = new Vector3 (transform.position.x - center.x, transform.position.y - center.y, twoD ? 0 : transform.position.z - center.z);

			transform.position = center;

			foreach (Transform t in transform) {
				t.position = new Vector3 (t.position.x + diff.x, t.position.y + diff.y, t.position.z + diff.z);
			}
		}
	}
}