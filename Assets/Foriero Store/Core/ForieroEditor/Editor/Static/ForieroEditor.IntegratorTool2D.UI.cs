using UnityEngine;
using System.IO;
using ForieroEditor.Extensions;
using System.Collections.Generic;
using UnityEditor;
using System.Xml.Linq;
using System.Linq;

using UnityEditor.SceneManagement;
using UnityEngine.UI;
using ForieroEngine.Extensions;

namespace ForieroEditor
{
	public static partial class IntegratorTool2DEditor
	{

		public static partial class RecreateScene
		{
			static Canvas canvas;
			static CanvasScaler canvasScaler;

			public static void GenerateHierarchyUI (string importUnityXml, string name)
			{

				TextAsset f = AssetDatabase.LoadAssetAtPath (importUnityXml, typeof(TextAsset)) as TextAsset;

				assetXMLFilePath = importUnityXml.Replace ("imports/import_unity.xml", "");

				if (!f) {
					Debug.LogError ("FILE NOT FOUND : " + importUnityXml);
					return;
				}
			
				try {
					XDocument doc = XDocument.Load (ForieroEditor.Extensions.ForieroEditorExtensions.GetMemoryStream (f.text));
			
					var ai2unity = doc.Element ("AI2UNITY");
			
					GameObject goRoot = new GameObject (Path.GetFileNameWithoutExtension (ai2unity.Attribute ("fileName").Value));
			
					canvas = goRoot.AddComponent<Canvas> ();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					canvasScaler = goRoot.AddComponent<CanvasScaler> ();
					canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
					canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
					goRoot.AddComponent<GraphicRaycaster> ();
			
					scaleFactor = float.Parse (ai2unity.Attribute ("scaleFactor").Value);
			
					illustratorDesigResolution = new Vector2 (
						float.Parse (ai2unity.Attribute ("illustratorDesignResolutionWidth").Value),
						float.Parse (ai2unity.Attribute ("illustratorDesignResolutionHeight").Value)
					);
								
					unityDesignResolution = new Vector2 (
						float.Parse (ai2unity.Attribute ("unityDesignResolutionWidth").Value),
						float.Parse (ai2unity.Attribute ("unityDesignResolutionHeight").Value)
					);
			
					scaleFactor *= canvasScaler.referenceResolution.x / unityDesignResolution.x;
																		
					var aiItems = ai2unity.Elements ();
					aiItems = aiItems.Reverse ();
					foreach (var aiItem in aiItems) {
						GenerateAIItemUI (aiItem, goRoot.transform, null);
					}
				} catch (System.Exception e) {
					Debug.LogError ("Invalid XML: " + importUnityXml + " " + e.Message);
				} finally {
					
				}
			}

			static void GenerateAIItemUI (XElement aiItem, Transform parent, XElement aiItemParent)
			{
				string name = aiItem.Attribute ("itemName").Value;
				string tag = aiItem.Attribute ("tag").Value;
			
				if (tag.Contains ("include") || HasChildInclude (aiItem)) {
					GameObject goItem = new GameObject (name);
					goItem.transform.SetParent (canvas.transform, true);

					RectTransform rt = goItem.AddComponent<RectTransform> ();
					rt.anchoredPosition3D = Vector3.zero;
					rt.SetSize (Vector2.zero);
			
					if (tag.Contains ("include")) {
						Image image = goItem.AddComponent<Image> ();
						image.sprite = GetAIPathSprite (aiItem);

						if (image.sprite.border != Vector4.zero) {
							image.type = Image.Type.Sliced;
						}

						image.SetNativeSize ();

						float imageScaleFactor = scaleFactor / (canvasScaler.referencePixelsPerUnit / image.sprite.pixelsPerUnit);

						image.rectTransform.SetSize (image.rectTransform.GetSize () * imageScaleFactor);

						SetAIItemPositionUI (aiItem, rt);
					}

					goItem.transform.SetParent (parent);
			
					var aiItems = aiItem.Elements ();

					aiItems = aiItems.Reverse ();

					foreach (var item in aiItems) {
						GenerateAIItemUI (item, goItem.transform, aiItem);
					}
				}
			}

			static void SetAIItemPositionUI (XElement aiItem, RectTransform rectTransform)
			{
				float x = float.Parse (aiItem.Attribute ("x").Value) * scaleFactor;
				float y = float.Parse (aiItem.Attribute ("y").Value) * scaleFactor;
				rectTransform.anchoredPosition = new Vector2 (-canvasScaler.referenceResolution.x / 2f + x, -canvasScaler.referenceResolution.y / 2f + y);
			}
		}
	}
}
