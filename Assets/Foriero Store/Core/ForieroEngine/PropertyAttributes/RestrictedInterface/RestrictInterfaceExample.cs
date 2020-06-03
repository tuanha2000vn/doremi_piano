using UnityEngine;
using UnityEngine.UI;

interface ITEST
{

}

public class RestrictInterfaceExample : MonoBehaviour, ITEST
{
	[RestrictInterface (typeof(ITEST))]
	public  Object[] m_LayoutElement;
}
