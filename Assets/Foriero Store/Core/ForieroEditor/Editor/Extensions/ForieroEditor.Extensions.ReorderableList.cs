using UnityEngine;
using UnityEditorInternal;
using System.Collections;

namespace ForieroEditor.Extensions
{
	public static partial class ForieroEditorExtensions
	{
		public static void SortReordableListAsString (this ReorderableList list, string propertyName)
		{
			var items = list.serializedProperty;
			GUI.FocusControl ("");
			for (int i = 0; i < items.arraySize; i++) {
				if (i < items.arraySize - 1) {
					if (items.GetArrayElementAtIndex (i).FindPropertyRelative (propertyName).stringValue.CompareTo (items.GetArrayElementAtIndex (i + 1).FindPropertyRelative (propertyName).stringValue) > 0) {
						if (list.index == i + 1) {
							list.index = i;
						} else if (list.index == i) {
							list.index = i + 1;
						}
						items.MoveArrayElement (i + 1, i);
						i = -1;
					}
				}
			}
			if (list.onSelectCallback != null) {
				list.onSelectCallback.Invoke (list);
			}
		}

		public static void SortReordableListAsInt (this ReorderableList list, string propertyName)
		{
			var items = list.serializedProperty;
			GUI.FocusControl ("");
			for (int i = 0; i < items.arraySize; i++) {
				if (i < items.arraySize - 1) {
					if (items.GetArrayElementAtIndex (i).FindPropertyRelative (propertyName).intValue.CompareTo (items.GetArrayElementAtIndex (i + 1).FindPropertyRelative (propertyName).intValue) > 0) {
						if (list.index == i + 1) {
							list.index = i;
						} else if (list.index == i) {
							list.index = i + 1;
						}
						items.MoveArrayElement (i + 1, i);
						i = -1;
					}
				}
			}
			if (list.onSelectCallback != null) {
				list.onSelectCallback.Invoke (list);
			}
		}
	}
}
