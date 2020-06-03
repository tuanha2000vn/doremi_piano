using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer (typeof(RestrictInterfaceAttribute))]
public class RestrictInterfacePropertyDrawer : PropertyDrawer
{

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		var restrictInterfaceAttribute = (RestrictInterfaceAttribute)attribute;
		if (SerializedPropertyType.ObjectReference == property.propertyType) {
			var propertyHeight = base.GetPropertyHeight (property, label);
			position.height = propertyHeight;

			//EditorGUIUtility.LookLikeControls ();
			EditorGUI.BeginChangeCheck ();
			var value = EditorGUI.ObjectField (position, label, property.objectReferenceValue, restrictInterfaceAttribute.RestrictType, true);
			if (EditorGUI.EndChangeCheck ()) {
				if (value) {
					if (value.GetType ().GetInterfaces ().Contains (restrictInterfaceAttribute.RestrictType)) {
						property.objectReferenceValue = value;
					} else {
						Debug.LogError (value.name + " does not implement interface " + restrictInterfaceAttribute.RestrictType.Name);
					}
				}
			}
		} else {
			EditorGUI.LabelField (position, label, new GUIContent ("This type has not supported."));
		}
	}
}
#endif

public class RestrictInterfaceAttribute : PropertyAttribute
{
	public System.Type RestrictType { get; set; }

	public RestrictInterfaceAttribute (System.Type restrictType)
	{
		RestrictType = restrictType;
	}
}
