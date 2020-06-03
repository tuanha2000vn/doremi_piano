using UnityEngine;

public class EnumLabelExample : MonoBehaviour
{

	public enum Example
	{
		[EnumLabel ("Marek")]
		HIGH,
		[EnumLabel ("Keram")]
		LOW
	}

	[EnumLabel ("例")]
	public Example test = Example.HIGH;
}
