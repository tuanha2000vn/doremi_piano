using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer (typeof(SortedEnumPopupAttribute))]
public class SortedEnumPopupDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
	{
		if (prop.propertyType == SerializedPropertyType.Enum) {
			string oldSelectedName = prop.enumNames [prop.enumValueIndex];
			string[] sortedNames = GetSortedNames (prop.enumNames);
			string newSelectedName;

			DrawPopup (position, label.text, oldSelectedName, sortedNames, out newSelectedName);
			prop.enumValueIndex = System.Array.IndexOf (prop.enumNames, newSelectedName);
		}
	}

	private void DrawPopup (Rect position, string labelName, string oldSelectedName, string[] names, out string newSelectedName)
	{
		int oldSelectedIndex = System.Array.IndexOf (names, oldSelectedName);
		int newSelectedIndex = EditorGUI.Popup (position, labelName, oldSelectedIndex, names);
		newSelectedName = names [newSelectedIndex];
	}

	private string[] GetSortedNames (string[] names)
	{
		string[] sortedNames = new string[names.Length];
		names.CopyTo (sortedNames, 0);
		System.Array.Sort (sortedNames);
		return sortedNames;
	}
}
#endif

public class SortedEnumPopupAttribute : PropertyAttribute
{
}