using UnityEngine;
using System.Collections;

namespace ForieroEngine.Extensions
{
	public enum ScreenStretch
	{
		None,
		Width,
		Height,
		Both,
		FitToWidth,
		FitToHeight
	}

	public static partial class ForieroEngineExtensions
	{

		/// <summary>
		/// Set sprite to screen size. 
		/// </summary>
		/// <param name="spriteRenderer">Sprite renderer.</param>
		/// <param name="screenStretch">None, Width, Height, Both</param> 
		/// <param name="scaler">Scale final localScale</param>
		/// <param name="camera">Orthographics Camera</param>
		public static void ResizeToScreen (this SpriteRenderer spriteRenderer, ScreenStretch screenStretch, Vector2 scaler, Camera camera)
		{

			if (spriteRenderer == null) {
				Debug.LogWarning ("SpriteRenderer is null");
				return;
			}

			if (!camera)
				camera = Camera.main;

			if (!camera.orthographic) {
				Debug.LogWarning ("Camera is not orthographics");
			}

			Vector3 localScale = spriteRenderer.transform.localScale;

			spriteRenderer.transform.localScale = Vector3.one;
			
			float width = spriteRenderer.sprite.bounds.size.x;
			float height = spriteRenderer.sprite.bounds.size.y;
			
			float worldScreenHeight = camera.orthographicSize * 2.0f;
			float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

			width = (worldScreenWidth / width) * scaler.x;
			height = (worldScreenHeight / height) * scaler.y;

			switch (screenStretch) {
			case ScreenStretch.None:
				break;
			case ScreenStretch.Width:
				spriteRenderer.transform.localScale = new Vector3 (
					width,
					localScale.y,
					localScale.z);
				break;
			case ScreenStretch.Height:
				spriteRenderer.transform.localScale = new Vector3 (
					localScale.x,
					height,
					localScale.z);
				break;
			case ScreenStretch.Both:
				spriteRenderer.transform.localScale = new Vector3 (
					width,
					height,
					localScale.z);
				break;
			case ScreenStretch.FitToWidth:
				spriteRenderer.transform.localScale = new Vector3 (
					width,
					width,
					localScale.z);
				break;
			case ScreenStretch.FitToHeight:
				spriteRenderer.transform.localScale = new Vector3 (
					height,
					height,
					localScale.z);
				break;
			}

		}
	}
}
