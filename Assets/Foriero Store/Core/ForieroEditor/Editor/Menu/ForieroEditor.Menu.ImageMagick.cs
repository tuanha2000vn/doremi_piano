using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

using Image = UnityEngine.UI.Image;

namespace ForieroEditor
{
    public static partial class Menu
    {

        [MenuItem("Assets/ImageMagick/Quantize")]
        public static void ImageMagickPngQuant()
        {
            switch (EditorUtility.DisplayDialogComplex("Quantize", "Quantize selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.PngQuantSelected("_quant");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.PngQuantSelected("");
                    break;
            }
        }

        static void ImageMagickJpgInternal(int quality)
        {
            switch (EditorUtility.DisplayDialogComplex("To Jpg", "Jpg selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.JpgSelected("_jpg", quality);
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.JpgSelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Jpg/10")]
        public static void ImageMagickJpg10()
        {
            ImageMagickJpgInternal(10);
        }

        [MenuItem("Assets/ImageMagick/Jpg/20")]
        public static void ImageMagickJpg20()
        {
            ImageMagickJpgInternal(20);
        }

        [MenuItem("Assets/ImageMagick/Jpg/30")]
        public static void ImageMagickJpg30()
        {
            ImageMagickJpgInternal(30);
        }

        [MenuItem("Assets/ImageMagick/Jpg/40")]
        public static void ImageMagickJpg40()
        {
            ImageMagickJpgInternal(40);
        }

        [MenuItem("Assets/ImageMagick/Jpg/50")]
        public static void ImageMagickJpg50()
        {
            ImageMagickJpgInternal(50);
        }

        [MenuItem("Assets/ImageMagick/Jpg/60")]
        public static void ImageMagickJpg60()
        {
            ImageMagickJpgInternal(60);
        }

        [MenuItem("Assets/ImageMagick/Jpg/70")]
        public static void ImageMagickJpg70()
        {
            ImageMagickJpgInternal(70);
        }

        [MenuItem("Assets/ImageMagick/Jpg/80")]
        public static void ImageMagickJpg80()
        {
            ImageMagickJpgInternal(80);
        }

        [MenuItem("Assets/ImageMagick/Jpg/90")]
        public static void ImageMagickJpg90()
        {
            ImageMagickJpgInternal(90);
        }

        [MenuItem("Assets/ImageMagick/Jpg/100")]
        public static void ImageMagickJpg100()
        {
            ImageMagickJpgInternal(100);
        }


        [MenuItem("Assets/ImageMagick/Drop Shadow")]
        public static void ImageMagickDropShadow()
        {
            switch (EditorUtility.DisplayDialogComplex("Drop Shadow", "Drow Shadow selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.DropShadowSelected("_shadow");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.DropShadowSelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Shadow Outlines")]
        public static void ImageMagickShadowOutlines()
        {
            switch (EditorUtility.DisplayDialogComplex("Shadow Outlines", "Shadow Outlines selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.DropShadowOutlineSelected("_shadowoutline");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.DropShadowOutlineSelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Soft Edges")]
        public static void ImageMagickSoftEdges()
        {
            switch (EditorUtility.DisplayDialogComplex("Soft Edges", "Soft Edges selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.SoftEdgesSelected("_softedges");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.SoftEdgesSelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Gray")]
        public static void ImageMagickGray()
        {
            switch (EditorUtility.DisplayDialogComplex("Gray", "Gray selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.GraySelected("_gray");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.GraySelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Blur")]
        public static void ImageMagickBlur()
        {
            switch (EditorUtility.DisplayDialogComplex("Blur", "Blur selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.BlurSelected("_blur");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.BlurSelected("");
                    break;
            }
        }


        [MenuItem("Assets/ImageMagick/Trim")]
        public static void ImageMagickTrim()
        {
            switch (EditorUtility.DisplayDialogComplex("Trim", "Trim selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
            {
                case 0:
                    Imagemagick.TrimSelected("_trim");
                    break;
                case 1:

                    break;
                case 2:
                    Imagemagick.TrimSelected("");
                    break;
            }
        }

        [MenuItem("Assets/ImageMagick/Watermark")]
        public static void ImageMagickWatermark()
        {
            string watermark = Path.Combine(Directory.GetCurrentDirectory(), "watermark.png");
            if (File.Exists(watermark))
            {
                switch (EditorUtility.DisplayDialogComplex("Watermark", "Watermark selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.WatermarkSelected(watermark, "_watermark");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.WatermarkSelected(watermark, "");
                        break;
                }
            }
            else
            {
                Debug.LogError("File not exists : " + watermark);
            }
        }
    }
}
