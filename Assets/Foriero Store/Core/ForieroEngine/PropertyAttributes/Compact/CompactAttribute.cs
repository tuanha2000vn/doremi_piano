using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System;

[CustomPropertyDrawer (typeof(CompactAttribute))]
public class CompactDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginChangeCheck ();
		switch (property.type) {
		case "Vector2":
			{
				Vector2 v2 = EditorGUI.Vector2Field (position, label.text, property.vector2Value);
				if (EditorGUI.EndChangeCheck ()) {
					property.vector2Value = v2;
				}
				break;
			}
		case "Vector3":
			{
				Vector3 v3 = EditorGUI.Vector3Field (position, label.text, property.vector3Value);
				if (EditorGUI.EndChangeCheck ()) {
					property.vector3Value = v3;
				}
				break;
			}
		case "Vector4":
			{
				Vector4 v4 = EditorGUI.Vector4Field (position, label.text, property.vector4Value);
				if (EditorGUI.EndChangeCheck ()) {
					property.vector4Value = v4;
				}
				break;
			}
		case "Rect":
			{
				Rect r = property.rectValue = EditorGUI.RectField (position, label.text, property.rectValue);
				if (EditorGUI.EndChangeCheck ()) {
					property.rectValue = r;
				}
				break;
			}
		case "Quaternion":
			{
				Quaternion q = property.quaternionValue;
				Vector4 v4 = EditorGUI.Vector4Field (position, label.text, new Vector4 (q.x, q.y, q.z, q.w));
				if (EditorGUI.EndChangeCheck ()) {
					property.quaternionValue = new Quaternion (v4.x, v4.y, v4.z, v4.w);
				}
				break;
			}
		case "Bounds":
			{
				Bounds b = EditorGUI.BoundsField (position, label, property.boundsValue);
				if (EditorGUI.EndChangeCheck ()) {
					property.boundsValue = b;
				}
				break;
			}
		default:
			{
				EditorGUI.LabelField (position, label.text, "Not Implemented");
				EditorGUI.EndChangeCheck ();
				break;
			}
		}

	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight (property, label) + GetHeight (property);
	}

	private const float SingleLineHeight = 16f;

	private static float GetHeight (SerializedProperty property)
	{
		float height = 0;
		switch (property.type) {
		case "Vector2":
		case "Vector3":
		case "Vector4":
		case "Quaternion":
			height = SingleLineHeight;
			break;
		case "Rect":
		case "Bounds":
			height = SingleLineHeight * 2;
			break;
		default:
			break;
		}
		return height;
	}

	private static SerializedProperty GetProperty (SerializedProperty property, string name)
	{
		return property.FindPropertyRelative (name);
	}
}
#endif

public class CompactAttribute : PropertyAttribute
{

}
