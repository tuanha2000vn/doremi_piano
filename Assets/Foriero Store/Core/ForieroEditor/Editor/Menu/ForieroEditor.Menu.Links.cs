using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace ForieroEditor
{
	public static partial class Menu
	{
		[MenuItem ("Foriero/Links/Portals/Amazon/Developer Portal")]
		public static void OpenAmazonDeveloperPortal ()
		{
			Application.OpenURL ("https://developer.amazon.com/home.html");
		}

		[MenuItem ("Foriero/Links/Portals/Apple/Certificates Portal")]
		public static void OpenAppleCertificatesPortal ()
		{
			Application.OpenURL ("https://developer.apple.com/account/overview.action");
		}

		[MenuItem ("Foriero/Links/Portals/Apple/iOS Developer Portal")]
		public static void OpeniOSDeveloperPortal ()
		{
			Application.OpenURL ("https://developer.apple.com/ios/");
		}

		[MenuItem ("Foriero/Links/Portals/Apple/OSX Developer Portal")]
		public static void OpenOSXDeveloperPortal ()
		{
			Application.OpenURL ("https://developer.apple.com/osx/");
		}

		[MenuItem ("Foriero/Links/Portals/Google Play/Developer Portal")]
		public static void OpenGooglePlayDeveloperPortal ()
		{
			Application.OpenURL ("https://play.google.com/apps/publish/");
		}

		[MenuItem ("Foriero/Links/Portals/Microsoft/Developer Portal")]
		public static void OpenMicrosoftDeveloperPortal ()
		{
			Application.OpenURL ("https://dev.windows.com/");
		}

		[MenuItem ("Foriero/Links/Portals/Samsung/Developer Portal")]
		public static void OpenSamsungDeveloperPortal ()
		{
			Application.OpenURL ("http://seller.samsungapps.com/");
		}

		[MenuItem ("Foriero/Links/Portals/Ubuntu/Developer Portal")]
		public static void OpenUbuntuDeveloperPortal ()
		{
			Application.OpenURL ("https://myapps.developer.ubuntu.com/");
		}

		//https://developers.facebook.com/

		[MenuItem ("Foriero/Links/Portals/Facebook/Developer Portal")]
		public static void OpenFacebookDeveloperPortal ()
		{
			Application.OpenURL ("https://developers.facebook.com/");
		}

		[MenuItem ("Foriero/Links/Others/SVN Command Line")]
		public static void OpenSVNCommandLineLink ()
		{
			Application.OpenURL ("https://subversion.apache.org/packages.html");
		}

		[MenuItem ("Foriero/Links/Others/ImageMagick")]
		public static void OpenImageMagickLink ()
		{
			Application.OpenURL ("http://www.imagemagick.org/");
		}

		[MenuItem ("Foriero/Links/Others/MacPorts")]
		public static void OpenMacPortsLink ()
		{
			Application.OpenURL ("https://www.macports.org/");
		}

		[MenuItem ("Foriero/Links/Spine/Editor")]
		public static void OpenSpineEditorLink ()
		{
			Application.OpenURL ("https://www.esotericsoftware.com/");
		}

		[MenuItem ("Foriero/Links/Spine/Runtime")]
		public static void OpenSpineRuntimesLink ()
		{
			Application.OpenURL ("https://github.com/EsotericSoftware/spine-runtimes");
		}
	}
}