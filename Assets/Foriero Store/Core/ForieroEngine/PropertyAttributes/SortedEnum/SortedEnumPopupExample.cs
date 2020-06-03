using UnityEngine;
using System.Collections;

public class SortedEnumPopupExample : MonoBehaviour
{

	public enum TestEnum
	{
		z,
		f,
		t,
		x
	}

	[SortedEnumPopup]
	public TestEnum testEnum;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
