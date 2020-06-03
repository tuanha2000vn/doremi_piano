using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

using Image = UnityEngine.UI.Image;

namespace ForieroEditor
{
	public static class Sprites
	{
		public static void SwapTo (string ending = "@1x")
		{
			SpriteRenderer[] spriteRenderers = Object.FindObjectsOfType<SpriteRenderer> ();

			foreach (SpriteRenderer sr in spriteRenderers) {
				string assetPath = AssetDatabase.GetAssetPath (sr.sprite);
				assetPath = Regex.Replace (assetPath, "@.x", ending);
				Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> (assetPath);
				if (sprite) {
					sr.sprite = sprite;
				}
			}

			UnityEngine.UI.Image[] images = Object.FindObjectsOfType<UnityEngine.UI.Image> ();

			foreach (UnityEngine.UI.Image image in images) {
				string assetPath = AssetDatabase.GetAssetPath (image.sprite);
				assetPath = Regex.Replace (assetPath, "@.x", ending);
				Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> (assetPath);
				if (sprite) {
					image.sprite = sprite;
				}
			}
		}
	}
}
