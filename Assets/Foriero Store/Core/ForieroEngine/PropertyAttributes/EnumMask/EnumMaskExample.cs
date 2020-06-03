using UnityEngine;
using System.Collections;

public class EnumMaskExample : MonoBehaviour
{
	[EnumMask]
	public Flags m_Flags;

	[System.Flags]
	public enum Flags
	{
		foo = 1 << 0,
		bar = 1 << 1,
		baz = 1 << 2,
		qux = 1 << 3,
	}
}
