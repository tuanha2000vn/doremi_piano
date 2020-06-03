using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

#pragma warning disable 414

[CustomEditor(typeof(AnimateSprite))]
public class AnimateSpriteEditor : Editor {
	private SerializedProperty _scenesProperty;

	private ReorderableList list;

	private AnimateSprite animateSprite;
	private float spritesPerSecond;
	private float spriteDuration;

	private void OnEnable() {
		animateSprite = target as AnimateSprite;

		spritesPerSecond = animateSprite.spritesPerSecond;
		spriteDuration = animateSprite.spriteDuration;

		list = new ReorderableList(serializedObject, 
		                           serializedObject.FindProperty("sprites"), 
		                           true, true, true, true);

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
		};

		list.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Sprites");
		};
	}
	
	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		animateSprite.spritesPerSecond = EditorGUILayout.FloatField("Sprites Per Second", animateSprite.spritesPerSecond);
		
		animateSprite.spriteDuration = EditorGUILayout.FloatField("Sprite Duration", animateSprite.spriteDuration);

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("Play")){
			animateSprite.Play();
		}

		if(GUILayout.Button ("Stop")){
			animateSprite.Stop();
		}

		if(GUILayout.Button ("Pause")){
			animateSprite.Pause();
		}
		EditorGUILayout.EndHorizontal();

		serializedObject.Update();

		list.DoLayoutList();
								
		serializedObject.ApplyModifiedProperties();
	}
}
